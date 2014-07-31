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
    public sealed partial class Home2 : Page
    {
        public Home2()
        {
            this.InitializeComponent();
            this.LoadData();

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #region Custom Code

        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage,
            HelpMessage
        };

        public void NotifyUser(string strMessage, NotifyType type)
        {
            if (StatusBlock != null)
            {
                switch (type)
                {
                    case NotifyType.StatusMessage:
                        StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                        break;
                    case NotifyType.ErrorMessage:
                        StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                        break;
                    case NotifyType.HelpMessage:
                        StatusBorder.Height = 200;
                        break;
                }
                StatusBlock.Text = strMessage;

                // Collapse the StatusBlock if it has no text to conserve real estate.
                if (StatusBlock.Text != String.Empty)
                {
                    StatusBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    StatusBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

            }
        }

        private void ClearNotification()
        {
            StatusBlock.Text = string.Empty;
            StatusBorder.Height = 20;
        }

        private void ClearFields()
        {

            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboCarBranch.Items.Clear);
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboCarModel.Items.Clear);
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.cboVersion.Items.Clear);

            this.cboCarBranch.ItemsSource = null;
            this.cboCarModel.ItemsSource = null;
            this.cboVersion.ItemsSource = null;

         
        }

        private void ClearCalculatedFields()
        {
            this.txtPrice.Text = "";
            this.txtIpva.Text = "";
            this.txtSeguro.Text = "";
            this.txtCustoOportunidade.Text = "";
            this.txtDepreciacao.Text = "";
            this.txtManutencao.Text = "";
            this.txtTotalAnual.Text = "";
            this.txtTotalMensal.Text = "";
        }

        private async void LoadData()
        {
            FipeDataSource c = new FipeDataSource();

            this.ClearFields();
            this.ClearCalculatedFields();

            var carBranches = await c.GetCarBranches();

            this.ClearNotification();

            this.cboCarBranch.ItemsSource = carBranches;

        }

        private async void LoadModel()
        {
            FipeDataSource c = new FipeDataSource();

            if (this.cboCarBranch.SelectedValue != null)
            {
                string selectedBranch = (this.cboCarBranch.SelectedValue as CarBranch).Id.ToString();

                var carModels = await c.GetModelByBranch(selectedBranch);

                this.cboCarModel.ItemsSource = carModels;

                this.ClearNotification();
               
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

                this.ClearNotification();
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
                && (this.cboVersion.SelectedValue != null))
            {
                selectedBranch = (this.cboCarBranch.SelectedValue as CarBranch).Id.ToString();

                selectedModel = (this.cboCarModel.SelectedValue as CarModel).Id.ToString();

                selectedVersion = (this.cboVersion.SelectedValue as CarModelYear).Id.ToString();


                var carDetails = await c.GetCarDetails(selectedBranch, selectedModel, selectedVersion);

                this.ClearNotification();

                if (carDetails != null)
                {
                    this.txtPrice.Text = carDetails.Preco;
                    this.Calculate(carDetails.Preco);
                }
            }

        }

        private void Calculate(string fullPrice)
        {
            const string CURRENCY_PREFIX = "R$";

            var cultureInfo = new System.Globalization.CultureInfo("pt-BR");
            double price = double.Parse(fullPrice, System.Globalization.NumberStyles.Currency, cultureInfo);
            double ipva = price * (4D / 100);
            double seguro = price * (8D / 100);
            double custoOportunidade = price * (6D / 100);
            double depreciacao = price * (5D / 100);
            double manutencao = price * (4D / 100);
            double totalAnual = ipva + seguro + custoOportunidade + depreciacao + manutencao;

            this.txtIpva.Text = string.Format("{0} {1}", CURRENCY_PREFIX, (ipva).ToString());
            this.txtSeguro.Text = string.Format("{0} {1}", CURRENCY_PREFIX, (seguro).ToString());
            this.txtCustoOportunidade.Text = string.Format("{0} {1}", CURRENCY_PREFIX, (custoOportunidade).ToString());
            this.txtDepreciacao.Text = string.Format("{0} {1}", CURRENCY_PREFIX, (depreciacao).ToString());
            this.txtManutencao.Text = string.Format("{0} {1}", CURRENCY_PREFIX, (manutencao).ToString());
            this.txtTotalAnual.Text = string.Format("{0} {1}", CURRENCY_PREFIX, (totalAnual).ToString());
            this.txtTotalMensal.Text = string.Format("{0} {1}", CURRENCY_PREFIX, (totalAnual / 12).ToString());
        }

        private void cboCarBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.cboCarModel.ItemsSource = null;
            this.cboCarModel.Items.Clear();
            this.ClearCalculatedFields();
            this.NotifyUser("Carregando dados...", NotifyType.StatusMessage);
            this.LoadModel();



        }

        private void cboCarModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.cboVersion.ItemsSource = null;
            this.cboVersion.Items.Clear();
            this.ClearCalculatedFields();
            this.NotifyUser("Carregando dados...", NotifyType.StatusMessage);
            this.LoadVersion();
        }

        private void cboVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ClearCalculatedFields();
            this.NotifyUser("Carregando dados...", NotifyType.StatusMessage);
            this.GetCarDetails();
        }

        #endregion

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("testando...", NotifyType.HelpMessage);
        }

        private void StatusBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ClearNotification();
        }
    }
}
