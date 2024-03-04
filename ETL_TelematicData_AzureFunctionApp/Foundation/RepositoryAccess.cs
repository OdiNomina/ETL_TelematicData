using Microsoft.Data.SqlClient;
using System.Data;

namespace ETL_TelematicData_AzureFunctionApp.Foundation
{
    public abstract class RepositoryAccess : IRepositoryAccess
    {
        internal static DateTime SelectLastAPIRequest(SqlConnection connection, string equipmentID)
        {
            try
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT LastAPIRequest FROM Equipment WHERE EquipmentID = @Value1;";

                    cmd.Parameters.AddWithValue("Value1", equipmentID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader.GetDateTime("LastAPIRequest");
                        else
                            throw new Exception("Can't get last API request from database");
                    }
                }
            }
            catch (Exception) { throw; }
        }

        internal static void UpdateLastAPIRequest(SqlConnection connection, string equipmentID, DateTime date)
        {
            try
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"UPDATE Equipment SET LastAPIRequest = @Value2 WHERE EquipmentID = @Value1;";

                    cmd.Parameters.AddWithValue("Value1", equipmentID);

                    if (date != DateTime.MinValue)
                        cmd.Parameters.AddWithValue("Value2", date);
                    else
                        cmd.Parameters.AddWithValue("Value2", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception) { throw; }
        }

        //#####################################################################################################################

        public abstract void UpSertTelematicData(SqlConnection connection, string equipmentID);
        public abstract void UpSertDailyReport(SqlConnection connection, string equipmentID);
        
        public virtual void UpSertTelematicData(SqlConnection connection, DateTime currentDateTo)
        {
            throw new NotImplementedException();
        }
        
        internal static T? SelectLastValue<T>(SqlConnection connection, string equipmentID, string tableName, string columnName) where T : RepositoryAccess
        {
            try
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"SELECT TOP 1 * FROM @Value1 WHERE EquipmentID = @Value2 ORDER BY Recordtime DESC;";

                    cmd.Parameters.AddWithValue("Value1", tableName);
                    cmd.Parameters.AddWithValue("Value2", equipmentID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (typeof(T) == typeof(string))
                            {
                                return reader.GetString(columnName) as T; 
                            }

                            if (typeof(T) == typeof(bool))
                            {
                                int columnIndex = reader.GetOrdinal(columnName);
                                byte[] buffer = new byte[1];
                                reader.GetBytes(columnIndex, 0, buffer, 0, buffer.Length);

                                if (buffer[0] == 1)
                                    return true as T;
                                if (buffer[0] == 0)
                                    return false as T;
                            }

                            if(typeof(T) == typeof(float))
                            {
                                return reader.GetFloat(columnName) as T;
                            }
                        }
                    }
                    return null;
                }
            }
            catch (Exception) { throw; }
        }
    }
}
