using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WpfApp4.Core;

namespace WpfApp4.Modules.Login;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _password = "";

    [RelayCommand]
    private void Login()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            MessageBox.Show("用户名或密码不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (UserName == "admin" && Password == "123456")
        {
            NavigationService.NavigateTo("MemberQuery");
        }
        else
        {
            MessageBox.Show("用户名或密码错误", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
