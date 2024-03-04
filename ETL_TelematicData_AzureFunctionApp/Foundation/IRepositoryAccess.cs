using Microsoft.Data.SqlClient;

namespace ETL_TelematicData_AzureFunctionApp.Foundation
{
    internal interface IRepositoryAccess
    {
        internal void UpSertTelematicData(SqlConnection connection, string EquipmentID);

        /// <summary>
        /// New equipments require a value in lastAPIRequest for the following time series API request.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="currentDateTo"></param>
        internal void UpSertTelematicData(SqlConnection connection, DateTime currentDateTo);

        internal void UpSertDailyReport(SqlConnection connection, string EquipmentID);
    }
}
