using ETL_TelematicData_AzureFunctionApp.Foundation;
using Microsoft.Data.SqlClient;


namespace ETL_TelematicData_AzureFunctionApp.Domain.EquipmentMgmt
{
    internal class Standard : IEquipmentManagement
    {
        public void ManageEquipment<T>(SqlConnection connection, List<T> listEquipment, string triggerDateTime) where T : IRepositoryAccess
        {
            try
            {
                DateTime currentDateTo = DateTime.Parse(triggerDateTime).ToUniversalTime().AddHours(-12);

                foreach (T obj in listEquipment)
                    obj.UpSertTelematicData(connection, currentDateTo);
            }
            catch (Exception) { throw; }
        }
    }
}
