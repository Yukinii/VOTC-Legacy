using System.Windows;
using VOTCClient.Pages;

namespace VOTCClient.Windows
{
    public partial class ScriptUploader
    {
        public ScriptUploader()
        {
            InitializeComponent();
            Content = new ScriptUploaderDragDrop();
            SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
