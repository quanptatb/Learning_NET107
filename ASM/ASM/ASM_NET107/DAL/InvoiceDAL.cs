using ASM_NET107.Models;
using Microsoft.Data.SqlClient;

namespace ASM_NET107.DAL
{
    public class InvoiceDAL
    {
        private readonly string _connectionString;
        public InvoiceDAL(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");
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
                    cmd.Parameters.AddWithValue("@Date", invoice.InvoiceDate);
                    cmd.ExecuteNonQuery();

                    // 2. Insert Details
                    foreach (var item in details)
                    {
                        string sqlDet = "INSERT INTO SalesInvoiceDetails(DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES(@DetId, @InvId, @ProdId, @UnitPrice, @Qty)";
                        SqlCommand cmdDet = new SqlCommand(sqlDet, conn, transaction);
                        cmdDet.Parameters.AddWithValue("@DetId", item.DetailID);
                        cmdDet.Parameters.AddWithValue("@InvId", item.InvoiceID);
                        cmdDet.Parameters.AddWithValue("@ProdId", item.ProductID);
                        cmdDet.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
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
