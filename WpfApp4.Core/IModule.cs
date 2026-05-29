using System.Windows;

namespace WpfApp4.Core;

public interface IModule
{
    string Name { get; }
    void Initialize();
    FrameworkElement GetView();
}
