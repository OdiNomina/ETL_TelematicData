using ETL_TelematicData_AzureFunctionApp.Foundation;
using Microsoft.Data.SqlClient;

namespace ETL_TelematicData_AzureFunctionApp.Domain.EquipmentMgmt
{
    internal interface IEquipmentManagement
    {
        internal void ManageEquipment<T>(SqlConnection connection, List<T> listEquipment, string triggerDateTime) where T : IRepositoryAccess;
    }
}
