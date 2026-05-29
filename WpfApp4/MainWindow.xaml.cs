using System;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using WpfApp4.Core;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        private readonly ModuleLoader _moduleLoader;
        private string? _currentModuleName;

        public MainWindow()
        {
            InitializeComponent();
            _moduleLoader = new ModuleLoader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules"));

            _moduleLoader.LoadExternalModules();

            WeakReferenceMessenger.Default.Register<NavigateMessage>(this, (r, m) =>
            {
                Dispatcher.Invoke(() => NavigateTo(m.ModuleName));
            });

            WeakReferenceMessenger.Default.Register<ModuleReloadedMessage>(this, (r, m) =>
            {
                Dispatcher.Invoke(() => OnModuleReloaded(m.ModuleName));
            });

            NavigateTo("Login");
        }

        private void NavigateTo(string? moduleName)
        {
            var module = _moduleLoader.GetModule(moduleName!);
            if (module != null)
            {
                _currentModuleName = moduleName;
                MainContent.Content = module.GetView();
            }
        }

        private void OnModuleReloaded(string? moduleName)
        {
            // 如果当前显示的模块被热更新，则刷新视图
            if (!string.IsNullOrEmpty(moduleName) && _currentModuleName == moduleName)
            {
                NavigateTo(moduleName);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _moduleLoader.StopWatching();
            base.OnClosed(e);
        }
    }
}