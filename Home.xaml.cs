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
using YourCarCost.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace YourCarCost
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        List<string> years = new List<string>
        {
            "2014","2013","2012"
        };

        public Home()
        {

            this.InitializeComponent();
            this.LoadData();
        }


        private async void ClearFilterLists()
        {

            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboCarBranch.Items.Clear);
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboCarModel.Items.Clear);
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboVersion.Items.Clear);
            

        }
        private async void LoadData()
        {
            FipeDataSource c = new FipeDataSource();

            this.ClearFilterLists();

            var carBranches = await c.GetCarBranches();

            //var carModels = await c.GetCarByBranch(carBranches.FirstOrDefault().Id.ToString());

            //var carDetails = await c.GetModelsAndYearsByCar(carBranches.FirstOrDefault().Id.ToString(), 
            //                                                carModels.FirstOrDefault().Id.ToString());

            //var carmodel = c.GetCarModelsAsync();
            this.cboCarBranch.ItemsSource = carBranches;

            //this.cboYear.ItemsSource = years;

        }

        private async void LoadModel()
        {
            FipeDataSource c = new FipeDataSource();

            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboCarModel.Items.Clear);
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboVersion.Items.Clear);

            if (this.cboCarBranch.SelectedValue != null)
            {
                string selectedBranch = (this.cboCarBranch.SelectedValue as CarBranch).Id.ToString();

                var carModels = await c.GetModelByBranch(selectedBranch);

                this.cboCarModel.ItemsSource = carModels;

                
            }
        }

        private async void LoadVersion()
        {
            FipeDataSource c = new FipeDataSource();
            string selectedBranch = string.Empty;
            string selectedModel = string.Empty;

            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboVersion.Items.Clear);

            if ((this.cboCarBranch.SelectedValue != null) && (this.cboCarModel.SelectedValue != null))
            {
                selectedBranch = (this.cboCarBranch.SelectedValue as CarBranch).Id.ToString();

                selectedModel = (this.cboCarModel.SelectedValue as CarModel).Id.ToString();

                var carVersions = await c.GetVersion(selectedBranch, selectedModel);

                this.cboVersion.ItemsSource = carVersions;
            }
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
            this.cboCarModel.Items.Clear();
            this.LoadModel();
            
            

        }

        private void cboCarModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.cboVersion.Items.Clear();
            this.LoadVersion();
        }

    }
}
