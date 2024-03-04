using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace ETL_TelematicData_AzureFunctionApp.Integration.TelematicData
{
    internal class SnapshotAPICall : TelematicDataIntegration
    {
        protected override void AddResponseDataToList<T>(ref List<T> objectList, JObject response, string nameOfDataToken)
        {
            try
            {
                JArray? inputJArray = response.SelectToken($"$.{nameOfDataToken}") as JArray;

                if (!inputJArray.IsNullOrEmpty())
                    foreach (JObject jOb in inputJArray)
                        objectList.Add(jOb.ToObject<T>());
            }
            catch (Exception) { throw; }
        }
    }
}
