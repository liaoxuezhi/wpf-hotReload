using System.Windows;
using WpfApp4.Core;

namespace WpfApp4.Modules.MemberQuery;

public class MemberQueryModule : IModule
{
    public string Name => "MemberQuery";

    public void Initialize() { }

    public FrameworkElement GetView() => new MemberQueryView();
}
