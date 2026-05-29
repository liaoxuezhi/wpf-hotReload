using CommunityToolkit.Mvvm.Messaging;

namespace WpfApp4.Core;

public class NavigateMessage
{
    public string? ModuleName { get; set; }
    public object? Parameter { get; set; }
}

public class ModuleReloadedMessage
{
    public string? ModuleName { get; set; }
}

public static class NavigationService
{
    public static void NavigateTo(string moduleName, object? parameter = null)
    {
        WeakReferenceMessenger.Default.Send(new NavigateMessage
        {
            ModuleName = moduleName,
            Parameter = parameter
        });
    }
}
