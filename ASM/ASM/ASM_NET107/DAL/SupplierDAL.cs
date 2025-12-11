using ASM_NET107.Models;
using Microsoft.Data.SqlClient;

namespace ASM_NET107.DAL
{
    public class SupplierDAL
    {
        private readonly string _connectionString;
        public SupplierDAL(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        public List<Suppliers> GetSuppliers()
        {
            var list = new List<Suppliers>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Suppliers", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Suppliers
                        {
                            SupplierID = reader["SupplierID"].ToString(),
                            SupplierName = reader["SupplierName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["CreatedDate"]))
                        });
                    }
                }
            }
            return list;
        }

        public void InsertSupplier(Suppliers supplier)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Suppliers (SupplierID, SupplierName, Email, Phone, CreatedDate, IsActive) VALUES (@Id, @Name, @Email, @Phone, @Date, @Active)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", GenerateSupplierID());
                cmd.Parameters.AddWithValue("@Name", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@Email", supplier.Email);
                cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@Active", true);
                cmd.ExecuteNonQuery();
            }
        }

        public string GenerateSupplierID()
        {
            string newID = "NCC001";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 SupplierID FROM Suppliers ORDER BY SupplierID DESC", conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string lastID = result.ToString();
                    int num = int.Parse(lastID.Substring(3)) + 1;
                    newID = "NCC" + num.ToString("D3");
                }
            }
            return newID;
        }
    }
}