using ASM_NET107.Models;
using Microsoft.Data.SqlClient;

namespace ASM_NET107.DAL
{
    public class SalesInvoiceDAL
    {
        private readonly string _connectionString;
        public SalesInvoiceDAL(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        public List<SalesInvoices> GetInvoices()
        {
            var list = new List<SalesInvoices>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM SalesInvoices", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SalesInvoices
                        {
                            InvoiceID = reader["InvoiceID"].ToString(),
                            CustomerID = reader["CustomerID"].ToString(),
                            EmployeeID = reader["EmployeeID"].ToString(),
                            InvoiceDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["InvoiceDate"])),
                            Status = Convert.ToBoolean(reader["Status"]),
                            TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0
                        });
                    }
                }
            }
            return list;
        }

        // Tạo hóa đơn mới
        public string InsertInvoice(SalesInvoices invoice)
        {
            string newID = GenerateInvoiceID();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, InvoiceDate, Status, TotalAmount) VALUES (@Id, @CusId, @EmpId, @Date, @Status, 0)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", newID);
                cmd.Parameters.AddWithValue("@CusId", invoice.CustomerID);
                cmd.Parameters.AddWithValue("@EmpId", invoice.EmployeeID);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@Status", true);
                cmd.ExecuteNonQuery();
            }
            return newID;
        }

        // Thêm chi tiết hóa đơn
        public void InsertInvoiceDetail(SalesInvoiceDetails detail)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES (@Id, @InvId, @ProdId, @Price, @Qty)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString()); // Hoặc tự sinh mã CT...
                cmd.Parameters.AddWithValue("@InvId", detail.InvoiceID);
                cmd.Parameters.AddWithValue("@ProdId", detail.ProductID);
                cmd.Parameters.AddWithValue("@Price", detail.UnitPrice);
                cmd.Parameters.AddWithValue("@Qty", detail.Quantity);
                cmd.ExecuteNonQuery();
            }
        }

        public string GenerateInvoiceID()
        {
            string newID = "HD001";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 InvoiceID FROM SalesInvoices ORDER BY InvoiceID DESC", conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string lastID = result.ToString();
                    int num = int.Parse(lastID.Substring(2)) + 1;
                    newID = "HD" + num.ToString("D3");
                }
            }
            return newID;
        }
        public void CreateInvoice(SalesInvoices invoice, List<SalesInvoiceDetails> details)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Nên dùng Transaction để đảm bảo tính toàn vẹn dữ liệu
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Insert Header
                    string sqlHead = "INSERT INTO SalesInvoices(InvoiceID, CustomerID, EmployeeID, InvoiceDate, Status) VALUES(@Id, @CusId, @EmpId, @Date, 1)";
                    SqlCommand cmd = new SqlCommand(sqlHead, conn, transaction);
                    // ... Add Parameters ...
                    cmd.Parameters.AddWithValue("@Id", invoice.InvoiceID);
                    cmd.Parameters.AddWithValue("@CusId", invoice.CustomerID);
                    cmd.Parameters.AddWithValue("@EmpId", invoice.EmployeeID);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmd.ExecuteNonQuery();

                    // 2. Insert Details
                    foreach (var item in details)
                    {
                        string sqlDet = "INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES (@Id, @InvId, @ProdId, @Price, @Qty)";
                        SqlCommand cmdDet = new SqlCommand(sqlDet, conn, transaction);
                        cmdDet.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                        cmdDet.Parameters.AddWithValue("@InvId", item.InvoiceID);
                        cmdDet.Parameters.AddWithValue("@ProdId", item.ProductID);
                        cmdDet.Parameters.AddWithValue("@Price", item.UnitPrice);
                        cmdDet.Parameters.AddWithValue("@Qty", item.Quantity);
                        cmdDet.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}