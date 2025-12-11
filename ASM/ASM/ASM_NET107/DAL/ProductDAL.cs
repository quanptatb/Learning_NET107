using ASM_NET107.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ASM_NET107.DAL
{
    public class ProductDAL
    {
        private readonly string _connectionString;

        public ProductDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Products> GetProducts()
        {
            var products = new List<Products>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Products
                        {
                            ProductID = reader["ProductID"].ToString(),
                            ProductName = reader["ProductName"].ToString(),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                            ImageURL = reader["ImageURL"] != DBNull.Value ? reader["ImageURL"].ToString() : null
                        });
                    }
                }
            }
            return products;
        }

        public Products GetProductById(string id)
        {
            Products product = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Products
                        {
                            ProductID = reader["ProductID"].ToString(),
                            ProductName = reader["ProductName"].ToString(),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                            ImageURL = reader["ImageURL"] != DBNull.Value ? reader["ImageURL"].ToString() : null
                        };
                    }
                }
            }
            return product;
        }

        public void InsertProduct(Products product)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Products (ProductID, ProductName, UnitPrice, StockQuantity, ImageURL) VALUES (@Id, @Name, @Price, @Stock, @Img)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", GenerateProductID());
                cmd.Parameters.AddWithValue("@Name", product.ProductName);
                cmd.Parameters.AddWithValue("@Price", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Stock", product.StockQuantity);
                cmd.Parameters.AddWithValue("@Img", (object)product.ImageURL ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateProduct(Products product)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "UPDATE Products SET ProductName = @Name, UnitPrice = @Price, StockQuantity = @Stock, ImageURL = @Img WHERE ProductID = @Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", product.ProductID);
                cmd.Parameters.AddWithValue("@Name", product.ProductName);
                cmd.Parameters.AddWithValue("@Price", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Stock", product.StockQuantity);
                cmd.Parameters.AddWithValue("@Img", (object)product.ImageURL ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteProduct(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ProductID = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public string GenerateProductID()
        {
            string newID = "SP001";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 ProductID FROM Products ORDER BY ProductID DESC", conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string lastID = result.ToString();
                    int num = int.Parse(lastID.Substring(2)) + 1;
                    newID = "SP" + num.ToString("D3");
                }
            }
            return newID;
        }

        public List<Products> SearchProducts(string keyword)
        {
            var products = new List<Products>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Tìm theo tên sản phẩm (có thể mở rộng tìm theo giá nếu cần)
                string sql = "SELECT * FROM Products WHERE ProductName LIKE @Keyword";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Products
                        {
                            ProductID = reader["ProductID"].ToString(),
                            ProductName = reader["ProductName"].ToString(),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                            ImageURL = reader["ImageURL"] != DBNull.Value ? reader["ImageURL"].ToString() : null
                        });
                    }
                }
            }
            return products;
        }
    }
}