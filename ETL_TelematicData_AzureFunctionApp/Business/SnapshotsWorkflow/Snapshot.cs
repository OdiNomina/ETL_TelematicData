using ETL_TelematicData_AzureFunctionApp.Domain.EquipmentMgmt;
using ETL_TelematicData_AzureFunctionApp.Foundation;
using ETL_TelematicData_AzureFunctionApp.Integration.TelematicData;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ETL_TelematicData_AzureFunctionApp.Business.SnapshotsWorkflow
{
    internal class Snapshot
    {
        public string AccessToken { get; set; } = "";
        public string APIConnectionURL { get; set; } = "";
        public string SQLConnectionString { get; set; } = "";
        public string TriggerDateTime { get; set; } = "";

        public ITelematicDataIntegration? APICallBehavior { get; set; }
        public IEquipmentManagement? EMBehavior { get; set; }
        public string NameOfDataToken { get; set; } = "";

        public string EndpointURL { get; set; } = "";

        protected async Task ReadFunctionParameters(HttpRequest postFromPipeline)
        {
            try
            {
                string parameterString = await postFromPipeline.ReadAsStringAsync();
                if(string.IsNullOrEmpty(parameterString)) throw new ArgumentException("No post from pipeline");
                
                JsonSerializerSettings settings = new() { DateParseHandling = DateParseHandling.None };
               
                JObject? parameters = JsonConvert.DeserializeObject<JObject>(parameterString, settings);

                if(parameters == null || string.IsNullOrEmpty(parameters["AccessToken"]?.ToString())) throw new ArgumentException("No Access Token");
                AccessToken = parameters["AccessToken"].ToString();
                    
                if (string.IsNullOrEmpty(parameters["APIConnectionURL"]?.ToString())) throw new ArgumentException("No APIConnectionURL");
                APIConnectionURL = parameters["APIConnectionURL"].ToString();

                if (string.IsNullOrEmpty(parameters["SQLConnectionString"]?.ToString())) throw new ArgumentException("No SQLConnectionString");
                SQLConnectionString = parameters["SQLConnectionString"].ToString();

                if (string.IsNullOrEmpty(parameters["TriggerDateTime"]?.ToString())) throw new ArgumentException("No DateTo");
                TriggerDateTime = parameters["TriggerDateTime"].ToString();

                //Endpoint URL could diverge
                EndpointURL = APIConnectionURL;
            }
            catch (Exception) { throw; }
        }

        protected async Task<List<T>> PerformAPICall<T>() where T : IRepositoryAccess
        {
            try
            {
                if (APICallBehavior == null) throw new ArgumentNullException();
                return await APICallBehavior.ReadAllData<T>(EndpointURL, AccessToken, NameOfDataToken);
            }
            catch (Exception) { throw; }
        }

        protected void PerformEMBehavior<T>(SqlConnection connection, List<T> ListEquipment) where T : IRepositoryAccess
        {
            try
            {
                if(EMBehavior == null) throw new ArgumentNullException();
                EMBehavior.ManageEquipment<T>(connection, ListEquipment, TriggerDateTime);
            }
            catch (Exception) { throw; }
        }
    }
}
