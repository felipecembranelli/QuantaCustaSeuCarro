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


        private void ClearFilterLists()
        {

            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboCarBranch.Items.Clear);
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboCarModel.Items.Clear);
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboVersion.Items.Clear);

            this.cboCarBranch.ItemsSource = null;
            this.cboCarModel.ItemsSource = null;
            this.cboVersion.ItemsSource = null;
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

            if ((this.cboCarBranch.SelectedValue != null) && (this.cboCarModel.SelectedValue != null))
            {
                selectedBranch = (this.cboCarBranch.SelectedValue as CarBranch).Id.ToString();

                selectedModel = (this.cboCarModel.SelectedValue as CarModel).Id.ToString();

                var carVersions = await c.GetVersion(selectedBranch, selectedModel);

                this.cboVersion.ItemsSource = carVersions;
            }
        }

        private async void GetCarDetails()
        {
            FipeDataSource c = new FipeDataSource();
            string selectedBranch = string.Empty;
            string selectedModel = string.Empty;
            string selectedVersion = string.Empty;

            if ((this.cboCarBranch.SelectedValue != null) 
                && (this.cboCarModel.SelectedValue != null)
                && (this.cboVersion.SelectedValue!=null))
            {
                selectedBranch = (this.cboCarBranch.SelectedValue as CarBranch).Id.ToString();

                selectedModel = (this.cboCarModel.SelectedValue as CarModel).Id.ToString();

                selectedVersion = (this.cboVersion.SelectedValue as CarModelYear).Id.ToString();


                var carDetails = await c.GetCarDetails(selectedBranch, selectedModel, selectedVersion);

                    if (carDetails!=null)
                    {
                        this.txtPrice.Text = carDetails.Preco;
                        this.Calculate(carDetails.Preco);
                    }
            }

        }

        private void Calculate(string fullPrice)
        {
            var cultureInfo = new System.Globalization.CultureInfo("pt-BR");
            double price = double.Parse(fullPrice, System.Globalization.NumberStyles.Currency, cultureInfo);


            this.txtIpva.Text = (price * (4D / 100)).ToString();
            this.txtSeguro.Text = (price * (8D / 100)).ToString();
            this.txtCustoOportunidade.Text = (price * (6D / 100)).ToString();
            this.txtDepreciacao.Text = (price * (5D / 100)).ToString();
            this.txtManutencao.Text = (price * (4D / 100)).ToString();


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
            this.cboCarModel.ItemsSource = null;
            this.cboCarModel.Items.Clear();
            this.LoadModel();
            
            

        }

        private void cboCarModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.cboVersion.ItemsSource = null;
            this.cboVersion.Items.Clear();
            this.LoadVersion();
        }

        private void cboVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GetCarDetails();
        }

    }
}
