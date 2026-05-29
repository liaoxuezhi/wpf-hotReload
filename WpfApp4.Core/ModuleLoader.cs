using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.Messaging;

namespace WpfApp4.Core;

public class ModuleLoader
{
    private readonly List<IModule> _modules = new();
    private readonly string _modulesDirectory;
    private FileSystemWatcher? _watcher;
    private readonly Dictionary<string, DateTime> _lastReloadTime = new();

    public IReadOnlyList<IModule> Modules => _modules;

    public ModuleLoader(string modulesDirectory)
    {
        _modulesDirectory = modulesDirectory;
    }

    public void LoadExternalModules()
    {
        if (!Directory.Exists(_modulesDirectory)) return;

        LoadDllFiles(Directory.GetFiles(_modulesDirectory, "*.dll"));
        StartWatching();
    }

    private void LoadDllFiles(string[] dllFiles)
    {
        foreach (var dll in dllFiles)
        {
            LoadDll(dll);
        }
    }

    private void LoadDll(string dllPath)
    {
        try
        {
            // 从内存加载 DLL，避免文件锁定，支持运行时删除
            var assemblyBytes = File.ReadAllBytes(dllPath);
            var assembly = Assembly.Load(assemblyBytes);
            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof(IModule).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract);

            foreach (var type in moduleTypes)
            {
                var module = Activator.CreateInstance(type) as IModule;
                if (module != null)
                {
                    module.Initialize();
                    _modules.Add(module);
                }
            }
        }
        catch
        {
            // 忽略加载失败的 DLL
        }
    }

    private void StartWatching()
    {
        _watcher = new FileSystemWatcher(_modulesDirectory)
        {
            Filter = "*.dll",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Renamed += OnFileRenamed;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        ReloadModule(e.FullPath);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        if (e.FullPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            ReloadModule(e.FullPath);
        }
        // 旧文件被重命名，移除对应模块
        if (e.OldFullPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            RemoveModulesByDll(e.OldFullPath);
        }
    }

    private void ReloadModule(string dllPath)
    {
        // 防抖：同一文件 500ms 内不重复处理
        var now = DateTime.UtcNow;
        if (_lastReloadTime.TryGetValue(dllPath, out var lastTime) && (now - lastTime).TotalMilliseconds < 500)
            return;
        _lastReloadTime[dllPath] = now;

        // 等待文件写入完成
        System.Threading.Thread.Sleep(100);

        // 移除旧模块
        var removedNames = RemoveModulesByDll(dllPath);

        // 加载新模块
        LoadDll(dllPath);

        // 通知 UI 刷新
        foreach (var name in removedNames)
        {
            WeakReferenceMessenger.Default.Send(new ModuleReloadedMessage { ModuleName = name });
        }
    }

    private List<string> RemoveModulesByDll(string dllPath)
    {
        var removedNames = new List<string>();
        var dllFileName = Path.GetFileNameWithoutExtension(dllPath);

        // 通过模块类型所在的程序集匹配
        for (int i = _modules.Count - 1; i >= 0; i--)
        {
            var moduleType = _modules[i].GetType();
            if (moduleType.Assembly.GetName().Name == dllFileName)
            {
                removedNames.Add(_modules[i].Name);
                _modules.RemoveAt(i);
            }
        }

        return removedNames;
    }

    public IModule? GetModule(string name)
    {
        return _modules.FirstOrDefault(m => m.Name == name);
    }

    public void StopWatching()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnFileChanged;
            _watcher.Created -= OnFileChanged;
            _watcher.Renamed -= OnFileRenamed;
            _watcher.Dispose();
            _watcher = null;
        }
    }
}
