using System.Windows.Controls;

namespace WpfApp4.Modules.MemberQuery;

public partial class MemberQueryView : UserControl
{
    public MemberQueryView()
    {
        InitializeComponent();
        DataContext = new MemberQueryViewModel();
    }
}
