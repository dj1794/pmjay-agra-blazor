using System.Data;
using Microsoft.Data.SqlClient;

namespace PmjayAgra.Blazor.Data;

public class AgraDataService
{
 private readonly string _connectionString;
 public AgraDataService(IConfiguration config)
 {
 _connectionString = config.GetConnectionString("Default") ?? Environment.GetEnvironmentVariable("DB_CONNECTION") ?? "Server=DINESHJ\\SQLSERVERDJ;Database=GauravLab;User Id=Rapid;Password=Rapid@123;TrustServerCertificate=True;";
 }

 public async Task<int> GetTotalAsync()
 {
 using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT COUNT(*) FROM [Agra1];";
 var obj = await cmd.ExecuteScalarAsync();
 return Convert.ToInt32(obj);
 }

 public async Task<List<Dictionary<string, object?>>> GetPageAsync(int page, int pageSize)
 {
 int offset = (page -1) * pageSize;
 using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT * FROM [Agra1] ORDER BY (SELECT NULL) OFFSET @offset ROWS FETCH NEXT @ps ROWS ONLY;";
 var p1 = cmd.CreateParameter(); p1.ParameterName = "@offset"; p1.Value = offset; p1.DbType = DbType.Int32; cmd.Parameters.Add(p1);
 var p2 = cmd.CreateParameter(); p2.ParameterName = "@ps"; p2.Value = pageSize; p2.DbType = DbType.Int32; cmd.Parameters.Add(p2);
 using var rdr = await cmd.ExecuteReaderAsync();
 var results = new List<Dictionary<string, object?>>();
 while (await rdr.ReadAsync())
 {
 var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
 for (int i =0; i < rdr.FieldCount; i++)
 {
 row[rdr.GetName(i)] = rdr.IsDBNull(i) ? null : rdr.GetValue(i);
 }
 results.Add(row);
 }
 return results;
 }
}
