using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Bai02.Models;

namespace Bai02.DAL
{
    public class ProductDAL
    {
        private readonly string _connectionString;

        public ProductDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Lấy tất cả sản phẩm cho trang Index
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Products";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Description = reader["Description"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Category = reader["Category"].ToString()
                    });
                }
            }
            return products;
        }

        // Lấy chi tiết 1 sản phẩm theo ID
        public Product GetProductById(int id)
        {
            Product product = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Products WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product = new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Description = reader["Description"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Category = reader["Category"].ToString()
                    };
                }
            }
            return product;
        }
    }
}
