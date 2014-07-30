using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace YourCarCost
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home1 : Page
    {
        public Home1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

           private void cboCarBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.cboCarModel.ItemsSource = null;
            //this.cboCarModel.Items.Clear();
            //this.LoadModel();
            
            

        }

        private void cboCarModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.cboVersion.ItemsSource = null;
            //this.cboVersion.Items.Clear();
            //this.LoadVersion();
        }

        private void cboVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.GetCarDetails();
        }
    }
}
