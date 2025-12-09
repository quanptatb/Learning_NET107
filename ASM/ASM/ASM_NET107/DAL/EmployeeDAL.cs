using Microsoft.Data.SqlClient;
using ASM_NET107.Models;

namespace ASM_NET107.DAL
{
    public class EmployeeDAL
    {
        private readonly string _connectionString;
        public EmployeeDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public List<Employees> GetEmployees()
        {
            List<Employees> employees = new List<Employees>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT EmployeeID, FullName, Email, Phone, Username, PasswordHash, Role, CreatedDate, IsActive FROM Employees";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employees employee = new Employees
                            {
                                EmployeeID = reader["EmployeeID"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Role = reader["Role"].ToString(),
                                CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["CreatedDate"])),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            return employees;
        }
        public List<Employees> GetActiveEmployees()
        {
            List<Employees> employees = new List<Employees>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT EmployeeID, FullName, Email, Phone, Username, PasswordHash, Role, CreatedDate, IsActive FROM Employees WHERE IsActive = 1";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employees employee = new Employees
                            {
                                EmployeeID = reader["EmployeeID"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Role = reader["Role"].ToString(),
                                CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["CreatedDate"])),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            return employees;
        }
        //get by id
        public Employees GetEmployeeById(string employeeID)
        {
            Employees employee = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT EmployeeID, FullName, Email, Phone, Username, PasswordHash, Role, CreatedDate, IsActive " +
                             "FROM Employees WHERE EmployeeID = @EmployeeID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            employee = new Employees
                            {
                                EmployeeID = reader["EmployeeID"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Role = reader["Role"].ToString(),
                                CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["CreatedDate"])),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            return employee;
        }

        //insert
        public void InsertEmployee(Employees employee)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Employees (EmployeeID, FullName, Email, Phone, Username, PasswordHash, Role, CreatedDate, IsActive) " +
                             "VALUES (@EmployeeID, @FullName, @Email, @Phone, @Username, @PasswordHash, @Role, @CreatedDate, @IsActive)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    //mã nv tự generate
                    string newID = GenerateEmployeeID();
                    command.Parameters.AddWithValue("@EmployeeID", newID);
                    command.Parameters.AddWithValue("@FullName", employee.FullName);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@Phone", employee.Phone);
                    command.Parameters.AddWithValue("@Username", employee.Username);
                    command.Parameters.AddWithValue("@PasswordHash", employee.PasswordHash);
                    command.Parameters.AddWithValue("@Role", employee.Role);
                    command.Parameters.AddWithValue("@CreatedDate", employee.CreatedDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@IsActive", employee.IsActive);
                    command.ExecuteNonQuery();
                }
            }
        }
        //tự động sinh mã nhân viên
        public string GenerateEmployeeID()
        {
            string newEmployeeID = "EMP001";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT TOP 1 EmployeeID FROM Employees ORDER BY EmployeeID DESC";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        string lastEmployeeID = result.ToString();
                        int numericPart = int.Parse(lastEmployeeID.Substring(3));
                        numericPart++;
                        newEmployeeID = "EMP" + numericPart.ToString("D3");
                    }
                }
            }
            return newEmployeeID;
        }

        //update
        public void UpdateEmployee(Employees employee)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE Employees SET FullName = @FullName, Email = @Email, Phone = @Phone, " +
                             "Username = @Username, PasswordHash = @PasswordHash, Role = @Role, " +
                             "CreatedDate = @CreatedDate, IsActive = @IsActive WHERE EmployeeID = @EmployeeID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                    command.Parameters.AddWithValue("@FullName", employee.FullName);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@Phone", employee.Phone);
                    command.Parameters.AddWithValue("@Username", employee.Username);
                    command.Parameters.AddWithValue("@PasswordHash", employee.PasswordHash);
                    command.Parameters.AddWithValue("@Role", employee.Role);
                    command.Parameters.AddWithValue("@CreatedDate", employee.CreatedDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@IsActive", employee.IsActive);
                    command.ExecuteNonQuery();
                }
            }
        }
        //delete
        public void DeleteEmployee(string employeeID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.ExecuteNonQuery();
                }
            }
        }
        //search by all
        public List<Employees> SearchEmployees(string keyword)
        {
            List<Employees> employees = new List<Employees>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT EmployeeID, FullName, Email, Phone, Username, PasswordHash, Role, CreatedDate, IsActive " +
                             "FROM Employees WHERE FullName LIKE @Keyword OR Email LIKE @Keyword OR Phone LIKE @Keyword " +
                             "OR Username LIKE @Keyword OR Role LIKE @Keyword";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employees employee = new Employees
                            {
                                EmployeeID = reader["EmployeeID"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Role = reader["Role"].ToString(),
                                CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["CreatedDate"])),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            return employees;
        }
    }
}
