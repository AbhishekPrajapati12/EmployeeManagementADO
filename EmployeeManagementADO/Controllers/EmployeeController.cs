
using EmployeeManagementADO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagementADO.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string cs;

        public EmployeeController(IConfiguration config)
        {
            cs = config.GetConnectionString("DB")!;
        }

        // Employee Page
        public IActionResult Index()
        {
            ViewBag.Departments = GetDepartments();
            return View();
        }

        // Get Departments
        public DataTable GetDepartments()
        {
            DataTable dt = new DataTable();

            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand("sp_GetDepartments", con);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        // Display and Search Employees
        public IActionResult List(string search = "")
        {
            List<Employee> list = new List<Employee>();

            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand("sp_GetEmployees", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Search", search);

            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Employee
                {
                    EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                    Name = dr["Name"].ToString()!,
                    Gender = dr["Gender"].ToString()!,
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentName = dr["DepartmentName"].ToString()!,
                    State = dr["State"].ToString()!,
                    City = dr["City"].ToString()!,
                    Salary = Convert.ToDecimal(dr["Salary"])
                });
            }

            return Json(list);
        }

        // Get Single Employee
        public IActionResult Get(int id)
        {
            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand("sp_GetEmployeeById", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeId", id);

            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                Employee e = new Employee
                {
                    EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                    Name = dr["Name"].ToString()!,
                    Gender = dr["Gender"].ToString()!,
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    State = dr["State"].ToString()!,
                    City = dr["City"].ToString()!,
                    Salary = Convert.ToDecimal(dr["Salary"])
                };

                return Json(e);
            }

            return NotFound();
        }

        // Add Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Please enter valid details" });
            }

            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand("sp_AddEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            AddParameters(cmd, employee);

            con.Open();
            cmd.ExecuteNonQuery();

            return Json(new { message = "Employee Added Successfully" });
        }

        // Update Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Please enter valid details" });
            }

            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);

            AddParameters(cmd, employee);

            con.Open();
            cmd.ExecuteNonQuery();

            return Json(new { message = "Employee Updated Successfully" });
        }

        // Common Parameters
        private void AddParameters(SqlCommand cmd, Employee e)
        {
            cmd.Parameters.AddWithValue("@Name", e.Name);
            cmd.Parameters.AddWithValue("@Gender", e.Gender);
            cmd.Parameters.AddWithValue("@DepartmentId", e.DepartmentId);
            cmd.Parameters.AddWithValue("@State", e.State);
            cmd.Parameters.AddWithValue("@City", e.City);
            cmd.Parameters.AddWithValue("@Salary", e.Salary);
        }

        // Delete Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand("sp_DeleteEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeId", id);

            con.Open();
            cmd.ExecuteNonQuery();

            return Json(new { message = "Employee Deleted Successfully" });
        }
    }
}
