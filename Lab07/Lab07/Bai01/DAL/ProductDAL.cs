using Bai01.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Bai01.DAL
{
    public class ProductDAL
    {
        private readonly string _connectionString;

        public ProductDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Lấy danh sách sản phẩm (Slide 7 - Trang 21)
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "",
                        Color = reader["Color"] != DBNull.Value ? reader["Color"].ToString() : "",
                        UnitPrice = (decimal)reader["UnitPrice"],
                        AvailableQuantity = (int)reader["AvailableQuantity"],
                        CreatedDate = (DateTime)reader["CreatedDate"]
                    });
                }
            }
            return products;
        }

        // Thêm sản phẩm (Slide 7 - Trang 22)
        public void AddProduct(Product product)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Products (Name, Category, Color, UnitPrice, AvailableQuantity, CreatedDate) VALUES (@Name, @Category, @Color, @UnitPrice, @AvailableQuantity, @CreatedDate)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Category", product.Category ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Color", product.Color ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@AvailableQuantity", product.AvailableQuantity);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        // Lấy sản phẩm theo Id (Slide 7 - Trang 23)
        public Product GetProductById(int id)
        {
            Product product = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product = new Product
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Category = reader["Category"].ToString(),
                        Color = reader["Color"].ToString(),
                        UnitPrice = (decimal)reader["UnitPrice"],
                        AvailableQuantity = (int)reader["AvailableQuantity"],
                        CreatedDate = (DateTime)reader["CreatedDate"]
                    };
                }
            }
            return product;
        }

        // Cập nhật sản phẩm (Slide 7 - Trang 22)
        public void UpdateProduct(Product product)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Products SET Name=@Name, Category=@Category, Color=@Color, UnitPrice=@UnitPrice, AvailableQuantity=@AvailableQuantity WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", product.Id);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Category", product.Category);
                cmd.Parameters.AddWithValue("@Color", product.Color);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@AvailableQuantity", product.AvailableQuantity);
                cmd.ExecuteNonQuery();
            }
        }

        // Xóa sản phẩm (Slide 7 - Trang 23)
        public void DeleteProduct(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}