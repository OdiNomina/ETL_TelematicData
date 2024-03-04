using Microsoft.Data.SqlClient;
using System.Data;

namespace ETL_TelematicData_AzureFunctionApp.Foundation.DataModels
{
    internal class Equipment : RepositoryAccess
    {
        internal const string TableName = "Equipment";
        public string? EquipmentID { get; set; }
        public string? OEMName { get; set; }
        public string? Model { get; set; }
        

        public override void UpSertTelematicData(SqlConnection connection, DateTime currentDateTo)
        {
            try
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = 
                    $"IF NOT EXISTS(SELECT 1 FROM {TableName} WHERE EquipmentID = @Value1) " +
                        "BEGIN " +
                            $"INSERT INTO {TableName} (EquipmentID, OEMName, Model, LastAPIRequest) " +
                                "VALUES (@Value1, @Value2, @Value3, @Value4) " +
                        "END " +
                    "ELSE " +
                        "BEGIN " +
                            $"UPDATE {TableName} SET OEMName = @Value2, Model = @Value3 WHERE EquipmentID = @Value1 " +
                        "END;";
                    
                    cmd.Parameters.AddWithValue("Value1", EquipmentID);

                    if (!string.IsNullOrEmpty(OEMName))
                        cmd.Parameters.AddWithValue("Value2", OEMName);
                        else
                            cmd.Parameters.AddWithValue("Value2", DBNull.Value);

                    if (!string.IsNullOrEmpty(Model))
                        cmd.Parameters.AddWithValue("Value3", Model);
                        else
                            cmd.Parameters.AddWithValue("Value3", DBNull.Value);

                    cmd.Parameters.AddWithValue("Value4", currentDateTo);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception) { throw; }
        }

        public override void UpSertTelematicData(SqlConnection connection, string equipmentID)
        {
            throw new NotImplementedException();
        }

        public override void UpSertDailyReport(SqlConnection connection, string equipmentID)
        {
            throw new NotImplementedException();
        }
    }
}
