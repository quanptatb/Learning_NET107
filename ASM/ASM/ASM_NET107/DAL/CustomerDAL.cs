using ASM_NET107.Models;
using Microsoft.Data.SqlClient;

namespace ASM_NET107.DAL
{
    public class CustomerDAL
    {
        private readonly string _connectionString;
        public CustomerDAL(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        public List<Customers> GetCustomers()
        {
            var list = new List<Customers>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Customers", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Customers
                        {
                            CustomerID = reader["CustomerID"].ToString(),
                            CustomerName = reader["CustomerName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["CreatedDate"]))
                        });
                    }
                }
            }
            return list;
        }

        public void InsertCustomer(Customers customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Customers (CustomerID, CustomerName, Phone, CreatedDate) VALUES (@Id, @Name, @Phone, @Date)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", GenerateCustomerID());
                cmd.Parameters.AddWithValue("@Name", customer.CustomerName);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        // Thêm Update, Delete, GetById tương tự như ProductDAL...

        public string GenerateCustomerID()
        {
            string newID = "KH001";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 CustomerID FROM Customers ORDER BY CustomerID DESC", conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string lastID = result.ToString();
                    int num = int.Parse(lastID.Substring(2)) + 1;
                    newID = "KH" + num.ToString("D3");
                }
            }
            return newID;
        }
        // Đăng ký (Thêm khách hàng mới)
        public void Register(Customers c)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Customers(CustomerID, CustomerName, Phone, Password, CreatedDate) VALUES(@Id, @Name, @Phone, @Pass, @Date)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString().Substring(0, 5)); // Demo sinh ID
                cmd.Parameters.AddWithValue("@Name", c.CustomerName);
                cmd.Parameters.AddWithValue("@Phone", c.Phone);
                cmd.Parameters.AddWithValue("@Pass", c.Password); // Lưu ý: Nên mã hóa password trong thực tế
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        // Đăng nhập khách hàng
        public Customers CheckLogin(string phone, string password)
        {
            Customers cus = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Query kiểm tra Phone và Password
                string sql = "SELECT * FROM Customers WHERE Phone = @Phone AND Password = @Pass";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Pass", password);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cus = new Customers
                        {
                            CustomerID = reader["CustomerID"].ToString(),
                            CustomerName = reader["CustomerName"].ToString(),
                            Phone = reader["Phone"].ToString()
                        };
                    }
                }
            }
            return cus;
        }
        public void UpdateCustomer(Customers c)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "UPDATE Customers SET CustomerName = @Name, Password = @Pass WHERE Phone = @Phone";
                // Lưu ý: Where theo Phone hoặc ID tùy vào cách bạn quản lý session (đang lưu Username là tên hay phone?)
                // Tốt nhất nên lưu CustomerID vào Session khi Login để update chính xác.

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name", c.CustomerName);
                cmd.Parameters.AddWithValue("@Pass", c.Password);
                cmd.Parameters.AddWithValue("@Phone", c.Phone);
                cmd.ExecuteNonQuery();
            }
        }

        public Customers GetCustomerByPhone(string phone)
        {
            // Viết hàm select * from Customers where Phone = @phone để load thông tin lên form sửa
            // ... (Code tương tự GetProductById)
            Customers cus = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Customers WHERE Phone = @Phone";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Phone", phone);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cus = new Customers
                        {
                            CustomerID = reader["CustomerID"].ToString(),
                            CustomerName = reader["CustomerName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Password = reader["Password"].ToString(),
                            CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["CreatedDate"]))
                        };
                    }
                }
            }
        }
    }
}