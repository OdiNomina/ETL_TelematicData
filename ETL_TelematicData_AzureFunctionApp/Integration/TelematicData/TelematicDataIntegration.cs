using ETL_TelematicData_AzureFunctionApp.Foundation;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;


namespace ETL_TelematicData_AzureFunctionApp.Integration.TelematicData
{
    internal abstract class TelematicDataIntegration : ITelematicDataIntegration
    {
        protected abstract void AddResponseDataToList<T>(ref List<T> objectList, JObject response, string NameOfDataToken) where T : IRepositoryAccess;

        public async Task<List<T>> ReadAllData<T>(string endpointURL, string accessToken, string nameOfDataToken) where T : IRepositoryAccess
        {
            try
            {
                JObject response = await SendGetRequestAsync(accessToken, endpointURL);

                List<T> list = new List<T>();
                AddResponseDataToList(ref list, response, nameOfDataToken);
                return list;
            }
            catch (Exception) { throw; }
        }

        protected async Task<JObject> SendGetRequestAsync(string accessToken, string endpointURL)
        {
            try
            {
                string jsonInputString = "";

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{accessToken}");
                    // GET request
                    HttpResponseMessage response = await httpClient.GetAsync(endpointURL);
                    response.EnsureSuccessStatusCode();

                    jsonInputString = await response.Content.ReadAsStringAsync();
                }
                JObject responseObject = JObject.Parse(jsonInputString);
                return responseObject;
            }
            catch (HttpRequestException) { throw; }
            catch (Exception) { throw; }
        }
    }
}
