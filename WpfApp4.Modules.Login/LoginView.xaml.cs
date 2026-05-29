using System.Windows;
using System.Windows.Controls;

namespace WpfApp4.Modules.Login;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = new LoginViewModel();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = PasswordBox.Password;
            vm.LoginCommand.Execute(null);
        }
    }
}
