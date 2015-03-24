using System.IO;

namespace VOTCClient.Windows
{
    /// <summary>
    /// Interaction logic for Changelog.xaml
    /// </summary>
    public partial class Changelog
    {
        public Changelog()
        {
            InitializeComponent();
            Text.AppendText(File.ReadAllText("News.txt"));
        }
    }
}
