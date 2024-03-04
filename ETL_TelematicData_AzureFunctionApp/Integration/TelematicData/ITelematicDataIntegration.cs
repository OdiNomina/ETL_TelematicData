using ETL_TelematicData_AzureFunctionApp.Foundation;

namespace ETL_TelematicData_AzureFunctionApp.Integration.TelematicData
{
    internal interface ITelematicDataIntegration
    {
        internal Task<List<T>> ReadAllData<T>(string endpointURL, string accessToken, string nameOfDataToken) where T : IRepositoryAccess;
    }
}
