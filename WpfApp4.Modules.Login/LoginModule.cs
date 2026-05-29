using System.Windows;
using WpfApp4.Core;

namespace WpfApp4.Modules.Login;

public class LoginModule : IModule
{
    public string Name => "Login";

    public void Initialize() { }

    public FrameworkElement GetView() => new LoginView();
}
