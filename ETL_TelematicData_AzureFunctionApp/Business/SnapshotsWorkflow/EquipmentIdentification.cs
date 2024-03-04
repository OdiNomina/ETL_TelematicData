using ETL_TelematicData_AzureFunctionApp.Domain.EquipmentMgmt;
using ETL_TelematicData_AzureFunctionApp.Foundation.DataModels;
using ETL_TelematicData_AzureFunctionApp.Integration.TelematicData;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Functions.Worker;

namespace ETL_TelematicData_AzureFunctionApp.Business.SnapshotsWorkflow
{
    internal class EquipmentIdentification : Snapshot
    {
        public List<Equipment> ListEquipment { get; set; } = new List<Equipment>();

        private EquipmentIdentification()
        {
            NameOfDataToken = "Equipment";
            APICallBehavior = new SnapshotAPICall();
            EMBehavior = new Standard();
        }

        [Function("EquipmentIdentification")]
        public static async Task<JArray?> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest postFromPipeline, ILogger log)
        {
            try
            {
                EquipmentIdentification fct = new EquipmentIdentification();
                await fct.ReadFunctionParameters(postFromPipeline);

                fct.ListEquipment = await fct.PerformAPICall<Equipment>();
                if (fct.ListEquipment.Count > 0)
                {
                    using (SqlConnection connection = new SqlConnection(fct.SQLConnectionString))
                    {
                        connection.Open();
                        fct.PerformEMBehavior(connection, fct.ListEquipment);
                    }
                    return JArray.Parse(JsonConvert.SerializeObject(fct.ListEquipment));
                }
                return null;
            }
            catch (Exception e)
            {
                log.LogInformation("FleetSnapshot - Run", e.StackTrace, e.Message);
                return null;
            }
        }
    }
}
