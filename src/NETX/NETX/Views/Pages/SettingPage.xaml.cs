using CommunityToolkit.Mvvm.DependencyInjection;
using NETX.ViewModels;
using System.Windows.Controls;

namespace NETX.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingPage.xaml
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<SettingPageViewModel>();
        }
    }
}
