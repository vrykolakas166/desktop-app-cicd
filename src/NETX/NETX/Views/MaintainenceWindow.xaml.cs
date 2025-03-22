using NETX.ViewModels.Maintainence;
using System.Windows;

namespace NETX.Views
{
    /// <summary>
    /// Interaction logic for MaintainenceWindow.xaml
    /// </summary>
    public partial class MaintainenceWindow : Window
    {
        public bool JustClosed { get; set; }

        private readonly LogsViewModel? vm;

        public MaintainenceWindow()
        {
            InitializeComponent();
            JustClosed = false;
            Closed += MaintainenceWindow_Closed;

            vm = Resources["ViewModel"] as LogsViewModel;
            if (vm is not null)
            {
                vm.ScrollToLastRequested += ScrollToLast;
            }
        }

        private void ScrollToLast(object? o, EventArgs e)
        {
            if (LogEventDataGrid.Items.Count > 0)
            {
                var lastItem = LogEventDataGrid.Items[^1];
                LogEventDataGrid.ScrollIntoView(lastItem);
            }
        }

        private void MaintainenceWindow_Closed(object? sender, EventArgs e)
        {
            JustClosed = true;
        }
    }
}
