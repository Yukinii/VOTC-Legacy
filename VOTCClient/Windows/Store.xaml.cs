using System;
using System.ComponentModel;
using VOTCClient.Core;
using VOTCClient.Pages;

namespace VOTCClient.Windows
{
    public partial class Store
    {
        public Store()
        {
            InitializeComponent();
            Title = "VOTC Store";
            Kernel.StoreWindow = this;
            Content = new StoreUi();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Content.GetType() != typeof(StoreDetailView))
            {
                Kernel.StoreWindow = null;
                GC.Collect();
                return;
            }
            Title = "VOTC Store";
            e.Cancel = true;
            Content = new StoreUi();
            GC.Collect(10, GCCollectionMode.Forced);
        }
    }
}
