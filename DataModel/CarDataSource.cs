using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace YourCarCost
{
    public sealed class FipeDataSource
    {
        private static FipeDataSource _fipeDataSource = new FipeDataSource();

        public List<CarModel> CarModels
        {
            get { return this._carModels; }
        }
        public List<CarBranch> CarBranches
        {
            get { return this._carBranches; }
        }

        public List<CarModelYear> CarModelYears
        {
            get { return this._carModelsYears; }
        }

        //public List<CarBranch> CarBranches
        //{
        //    get { return this._carBranches; }
        //}

        private List<CarModel> _carModels = new List<CarModel>();
        private List<CarBranch> _carBranches = new List<CarBranch>();
        private List<CarModelYear> _carModelsYears = new List<CarModelYear>();

        //public async Task GetLocalCarModelsAsync()
        //{
        //    if (this._carModels.Count != 0)
        //        return;

        //    Uri dataUri = new Uri("ms-appx:///DataModel/CarModelsData.json");

        //    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
        //    string jsonText = await FileIO.ReadTextAsync(file);
        //    JsonObject jsonObject = JsonObject.Parse(jsonText);
        //    JsonArray jsonArray = jsonObject["Cars"].GetArray();

        //    foreach (JsonValue carValue in jsonArray)
        //    {
        //        JsonObject carObject = carValue.GetObject();
        //        CarModel car = new CarModel(carObject["UniqueId"].GetString(),
        //                                                    carObject["Title"].GetString());

        //        this.CarModels.Add(car);
        //    }
        //}

        /// <summary>
        /// Retorna as marcas de carros
        /// GET: http://fipeapi.appspot.com/api/1/carros/marcas.json
        /// </summary>
        /// <returns></returns>
        public async Task<List<CarBranch>> GetCarBranches()
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Uri resourceUri;
            string jsonText;
            List<CarBranch> carBranches = new List<CarBranch>();

            resourceUri = new Uri(" http://fipeapi.appspot.com/api/1/carros/marcas.json");

            Windows.Storage.Streams.IInputStream response = await httpClient.GetInputStreamAsync(resourceUri);


            using (StreamReader reader = new StreamReader(response.AsStreamForRead()))
            {
                jsonText = await reader.ReadToEndAsync();
            }

            
            JsonValue jsonObject = JsonValue.Parse(jsonText);

            JsonArray jsonArray = jsonObject.GetArray();

            foreach (JsonValue carBranchValue in jsonArray)
            {
                JsonObject branchObject = carBranchValue.GetObject();
                CarBranch branch = new CarBranch(branchObject["key"].GetString(),
                                                            branchObject["id"].GetNumber(),
                                                            branchObject["fipe_name"].GetString(),
                                                            branchObject["name"].GetString());

                carBranches.Add(branch);
            }

            return carBranches;
        }


        /// <summary>
        /// Retorna lista de modelos de uma determinada marca
        /// GET: http://fipeapi.appspot.com/api/1/carros/veiculos/21.json
        /// </summary>
        /// <returns></returns>
        public async Task<List<CarModel>> GetModelByBranch(string branchId)
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Uri resourceUri;
            string jsonText;
            List<CarModel> carModels = new List<CarModel>();


            string uri = string.Format("http://fipeapi.appspot.com/api/1/carros/veiculos/{0}.json", branchId);

            resourceUri = new Uri(uri);

            Windows.Storage.Streams.IInputStream response = await httpClient.GetInputStreamAsync(resourceUri);


            using (StreamReader reader = new StreamReader(response.AsStreamForRead()))
            {
                jsonText = await reader.ReadToEndAsync();
            }


            JsonValue jsonObject = JsonValue.Parse(jsonText);

            JsonArray jsonArray = jsonObject.GetArray();

            foreach (JsonValue item in jsonArray)
            {
                JsonObject jObj = item.GetObject();
                CarModel model = new CarModel(jObj["id"].GetString(),
                                                jObj["name"].GetString(),
                                                jObj["key"].GetString(),
                                                jObj["marca"].GetString(),
                                                jObj["fipe_name"].GetString(),
                                                jObj["fipe_marca"].GetString());

                carModels.Add(model);
            }

            return carModels;
        }

        /// <summary>
        /// Retorna os modelos e anos disponíveis para um determinado veículo
        /// GET: http://fipeapi.appspot.com/api/1/carros/veiculo/21/001267-0.json
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public async Task<List<CarModelYear>> GetVersion(string branchId, string carId)
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Uri resourceUri;
            string jsonText;
            List<CarModelYear> carModelYears = new List<CarModelYear>();


            string uri = string.Format("http://fipeapi.appspot.com/api/1/carros/veiculo/{0}/{1}.json", branchId, carId);

            resourceUri = new Uri(uri);

            Windows.Storage.Streams.IInputStream response = await httpClient.GetInputStreamAsync(resourceUri);


            using (StreamReader reader = new StreamReader(response.AsStreamForRead()))
            {
                jsonText = await reader.ReadToEndAsync();
            }


            JsonValue jsonObject = JsonValue.Parse(jsonText);

            JsonArray jsonArray = jsonObject.GetArray();

            foreach (JsonValue item in jsonArray)
            {
                JsonObject jObj = item.GetObject();
                CarModelYear modelYears = new CarModelYear(jObj["id"].GetString(),
                                                jObj["veiculo"].GetString(),
                                                jObj["key"].GetString(),
                                                jObj["marca"].GetString(),
                                                jObj["name"].GetString(),
                                                jObj["fipe_codigo"].GetString(),
                                                jObj["fipe_marca"].GetString());

                carModelYears.Add(modelYears);
            }

            return carModelYears;
        }

        public async Task<Car> GetCarDetails(string branchId, string carId, string versionId)
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Uri resourceUri;
            string jsonText;
            Car car = null;


            string uri = string.Format("http://fipeapi.appspot.com/api/1/carros/veiculo/{0}/{1}/{2}.json", branchId, carId, versionId);

            resourceUri = new Uri(uri);

            Windows.Storage.Streams.IInputStream response = await httpClient.GetInputStreamAsync(resourceUri);


            using (StreamReader reader = new StreamReader(response.AsStreamForRead()))
            {
                jsonText = await reader.ReadToEndAsync();
            }


            JsonValue jsonObject = JsonValue.Parse(jsonText);

            //JsonArray jsonArray = jsonObject.GetArray();

            //foreach (JsonValue item in jsonArray)
            //{
                JsonObject jObj = jsonObject.GetObject();
                car = new Car(jObj["key"].GetString(),
                                                jObj["id"].GetString(),
                                                jObj["fipe_marca"].GetString(),
                                                jObj["referencia"].GetString(),
                                                jObj["fipe_codigo"].GetString(),
                                                jObj["preco"].GetString(),
                                                jObj["name"].GetString(),
                                                jObj["veiculo"].GetString(),
                                                jObj["marca"].GetString()
                                                );

            //}

            return car;
        }



    }


    /// <summary>
    /// Modelo
    /// </summary>
    public class CarModel
    {
        public CarModel(string id, string name, string key, string branch, string fipe_name, string fipe_marca)
        {
            this.Id = id;
            this.Name = name;
            this.key = key;
            this.Branch = branch;
            this.Fipe_name = fipe_name;
            this.Fipe_marca = fipe_marca;

        }

        public string Id { get; private set; }
        public string Name{ get; private set; }

        public string key { get; private set; }

        public string Branch { get; set; }

        public string Fipe_name { get; set; }

        public string Fipe_marca { get; set; }

        public override string ToString()
        {
            return this.Name;
        }



    }


    /// <summary>
    /// Modelo x Ano
    /// </summary>
    public class CarModelYear
    {
        public CarModelYear(string id, string veiculo, string key, string marca, string name, string fipe_codigo, string fipe_marca)
        {
            this.Id = id;
            this.Veiculo = veiculo;
            this.Key = key;
            this.Marca = marca;
            this.Name = name;
            this.Fipe_codigo = fipe_codigo;
            this.Fipe_marca = fipe_marca;

        }
        public string Id { get; private set; }

        public string Veiculo { get; private set; }
        public string Key { get; private set; }

        public string Marca { get; private set; }

        public string Name { get; set; }

        public string Fipe_codigo { get; set; }

        public string Fipe_marca { get; set; }

     
    }

    /// <summary>
    /// Marca
    /// </summary>
    public class CarBranch
    {

        public CarBranch(string key, double id, string fipe_name, string name)
        {
            this.Key = key;
            this.Id = id;
            this.Fipe_name = fipe_name;
            this.Name = name;
        }
        public string Key { get; private set; }
        public double Id { get; private set; }

        public string Fipe_name { get; private set; }

        public string Name { get; private set; }

    }

    public class Car
    {

        public Car(string key, string id, string fipe_marca, 
                            string referencia, string fipe_codigo, string preco, string name, string veiculo, string marca)
        {
            this.Key = key;
            this.Id = id;
            this.Fipe_marca = fipe_marca;
            this.Referencia = referencia;
            this.Fipe_codigo = fipe_codigo;
            this.Preco = preco;
            this.Veiculo = veiculo;
            this.Marca = marca;
            this.Name = name;
        }
        public string Key { get; private set; }
        public string Id { get; private set; }
        public string Fipe_marca { get; private set; }
        public string Referencia { get; private set; }
        public string Fipe_codigo { get; private set; }
        public string Preco { get; private set; }
        public string Name { get; private set; }
        public string Veiculo { get; private set; }
        public string Marca { get; private set; }

    }


}
