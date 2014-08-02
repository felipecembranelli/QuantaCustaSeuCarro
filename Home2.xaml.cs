using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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
        private DataTransferManager dataTransferManager;
        public const string MissingTitleError = "Entre um título para o que você está compartilhando.";

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
            // Register the current page as a share source.
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);

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
                        StatusBorder.Height = 100;
                        StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                        break;
                    case NotifyType.HelpMessage:
                        StatusBorder.Height = 100;
                        StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
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
            StatusBorder.Height = 50;
            StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
            StatusBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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

                if (carVersions.Count() == 0)
                    this.NotifyUser("Dados não disponíveis no site da FIPE.", NotifyType.ErrorMessage);
                else
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
            
        }

        private void StatusBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ClearNotification();
        }

        private void imgHelpFipe_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Preço médio da tabela FIPE.", NotifyType.HelpMessage);
        }

        private void imgHelpIpva_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Valor médio de IPVA pago anualmente.", NotifyType.HelpMessage);
        }

        private void imgHelpSeguro_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Valor médio do seguro do carro pago anualmente.", NotifyType.HelpMessage);
        }

        private void imgHelpManutencao_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Valor médio de gastos com manutenção anual do carro.", NotifyType.HelpMessage);
        }

        private void imgHelpDepreciacao_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Valor médio de desvalorização anual do carro.", NotifyType.HelpMessage);
        }

        private void imgHelpCustoOportunidade_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Valor que você poderia estar economizando se tivesse investido o mesmo valor do carro em outra alternativa de investimento, como por exemplo, na poupança (0,5% ao mês).", NotifyType.HelpMessage);
        }

        private void imgHelpTotalAnual_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Valor anual de todos os custos do seu carro.", NotifyType.HelpMessage);
        }

        private void imgHelpTotalMensal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.NotifyUser("Valor mensal de todos os custos do seu carro.", NotifyType.HelpMessage);
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            // If the user clicks the share button, invoke the share flow programatically.
            DataTransferManager.ShowShareUI();
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            if (GetShareContent(e.Request))
            {
                // Out of the datapackage properties, the title is required. If the scenario completed successfully, we need
                // to make sure the title is valid since the sample scenario gets the title from the user.
                if (String.IsNullOrEmpty(e.Request.Data.Properties.Title))
                {
                    e.Request.FailWithDisplayText(MissingTitleError);
                }
            }
        }

        private bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            // The URI used in this sample is provided by the user so we need to ensure it's a well formatted absolute URI
            // before we try to share it.
            this.NotifyUser("", NotifyType.StatusMessage);
            //Uri dataPackageUri = ValidateAndGetUri(this.adress);
            if (this.txtTotalMensal.Text == string.Empty)
            {
                DataPackage requestData = request.Data;
                requestData.Properties.Title = "Quanto custa o seu Carro?";
                requestData.Properties.Description = "Resultado do cálculo de quanto custa o seu carro"; // The description is optional.
                //requestData.Properties.ContentSourceApplicationLink = ApplicationLink;
                //requestData.SetWebLink(dataPackageUri);

                //string carro = string.Format("{0} - {1} - {2}",
                //                this.cboCarBranch.SelectedValue.ToString(),
                //                this.cboCarModel.SelectedValue.ToString(),
                //                this.cboVersion.SelectedValue.ToString());

                string carro = string.Format("{0}",
                                this.cboCarModel.SelectedValue.ToString());


                requestData.SetText(string.Format("O seu carro ({0}) está custando para você {1} / mês.",carro, this.txtTotalMensal.Text));
                //requestData.SetHtmlFormat()
                succeeded = true;
            }
            else
            {
                this.NotifyUser("Nenhum carro selecionado para compartilhar o cálculo.", NotifyType.ErrorMessage);
                //request.FailWithDisplayText("Nenhum carro selecionado para compartilhar o cálculo.");
            }
            return succeeded;
        }

   
    }

}
