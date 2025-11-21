using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rac.TestAutomation.Common.API
{
    public class Vehicle
    {
        private string _makeDescription;
        private string _modelDescription;
        private string _vehicleSubTypeDescription;

        [JsonProperty("makeDescription")]
        public string MakeDescription
        {
            get => _makeDescription;
            set => _makeDescription = string.IsNullOrEmpty(value) ? null : value.RemoveDuplicateWhiteSpaceAndTrim();
        }
        [JsonProperty("mainVehicleTypeDescription")]
        public string MainVehicleTypeDescription { get; set; }
        [JsonProperty("modelId")]
        public string ModelId { get; set; }
        [JsonProperty("modelFamily")]
        public string ModelFamily { get; set; }
        [JsonProperty("modelYearId")]
        public string ModelYearId { get; set; }
        [JsonProperty("mainVehicleTypeId")]
        public string MainVehicleTypeId { get; set; }
        [JsonProperty("transmissionId")]
        public string TransmissionId { get; set; }
        [JsonProperty("nvic")]
        public string Nvic { get; set; }
        [JsonProperty("transmissionDescription")]
        public string TransmissionDescription { get; set; }
        [JsonProperty("discontinueDate")]
        public object DiscontinueDate { get; set; }
        [JsonProperty("modelExternalCode")]
        public string ModelExternalCode { get; set; }
        [JsonProperty("price")]
        public int Price { get; set; }
        [JsonProperty("cylinderTypeExternalCode")]
        public string CylinderTypeExternalCode { get; set; }
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
        [JsonProperty("modelYearExternalCode")]
        public string ModelYearExternalCode { get; set; }
        [JsonProperty("makeId")]
        public string MakeId { get; set; }
        [JsonProperty("transmissionExternalCode")]
        public string TransmissionExternalCode { get; set; }
        [JsonProperty("cylinderTypeId")]
        public string CylinderTypeId { get; set; }
        [JsonProperty("vehicleSubTypeDescription")]
        public string VehicleSubTypeDescription
        {
            // Commonly hosts the "Body" type of the vehicle from Glasses.
            get => _vehicleSubTypeDescription;
            set => _vehicleSubTypeDescription = string.IsNullOrEmpty(value) ? null : value.RemoveDuplicateWhiteSpaceAndTrim();
        }
        [JsonProperty("mainVehicleTypeExternalCode")]
        public string MainVehicleTypeExternalCode { get; set; }
        [JsonProperty("modelYear")]
        public int ModelYear { get; set; }
        [JsonProperty("makeExternalCode")]
        public string MakeExternalCode { get; set; }
        [JsonProperty("modelDescription")]
        public string ModelDescription
        {
            get => _modelDescription; 
            set => _modelDescription = string.IsNullOrEmpty(value) ? null : value.RemoveDuplicateWhiteSpaceAndTrim();
        }
        [JsonProperty("ratingPoints")]
        public int RatingPoints { get; set; }
        [JsonProperty("vehicleSubTypeId")]
        public string VehicleSubTypeId { get; set; }
        [JsonProperty("vehicleSubTypeExternalCode")]
        public string VehicleSubTypeExternalCode { get; set; }
        [JsonProperty("engineSize")]
        public int EngineSize { get; set; }
        [JsonProperty("cylinderTypeDescription")]
        public string CylinderTypeDescription { get; set; }
    }

    public class GetVehicle_Response
    {
        [JsonProperty("vehicles")]
        public List<Vehicle> Vehicles { get; set; }
    }

    public class Manufacturer
    {
        [JsonProperty("externalCode")]
        public string ExternalCode { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("years")]
        public List<int> Years { get; set; }
    }

    public class GetManufacturers_Response
    {
        [JsonProperty("manufacturers")]
        public List<Manufacturer> Manufacturers { get; set; }
    }
}
