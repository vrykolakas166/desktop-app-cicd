using System.Windows;
using System.Windows.Controls;

namespace NETX.Views.Layout
{
    /// <summary>
    /// Interaction logic for ControlBar.xaml
    /// </summary>
    public partial class ControlBar : UserControl
    {
        // Define a Dependency Property
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ControlBar), new PropertyMetadata(string.Empty));

        public Visibility ThemeControlVisible
        {
            get { return (Visibility)GetValue(ThemeControlVisibleProperty); }
            set { SetValue(ThemeControlVisibleProperty, value); }
        }

        public static readonly DependencyProperty ThemeControlVisibleProperty =
            DependencyProperty.Register("ThemeControlVisible", typeof(Visibility), typeof(ControlBar), new PropertyMetadata(Visibility.Visible));


        public ControlBar()
        {
            InitializeComponent();
            MouseDown += ControlBar_MouseDown;
        }

        private void ControlBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow?.WindowState == WindowState.Maximized)
                {
                    parentWindow.WindowState = WindowState.Normal;
                    Point mousePosition = e.GetPosition(parentWindow);
                    parentWindow.Top = mousePosition.Y;
                    parentWindow.Left = mousePosition.X;
                }
                parentWindow?.DragMove();
            }
        }
    }
}
