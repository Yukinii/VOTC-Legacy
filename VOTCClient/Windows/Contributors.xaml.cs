using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace VOTCClient.Windows
{
    /// <summary>
    /// Interaction logic for Contributors.xaml
    /// </summary>
    public partial class Contributors : Window
    {
        public Contributors()
        {
            InitializeComponent();
        }

        private void ContentElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.facebook.com/groups/623473311006319/");
        }

        private void ContentElement_OnMouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://500px.com/movey11");
        }
    }
}
