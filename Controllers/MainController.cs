using Microsoft.AspNetCore.Mvc;
using HandBook.DataBaseFolder;
using System.Numerics;
using System.Text;
using System.Xml.Linq;

namespace HandBook.Controllers
{
    public class MainController : Controller
    {
        public IConfiguration server;
        static string user = "";
        public MainController(IConfiguration server)
        {
            this.server = server;
        }

        [HttpGet]
        public IActionResult Report(string selectOption = "", string selectValue = "")
        {
            int? numSelectionValue = null;
            if (selectOption == "id_department")
                numSelectionValue = Department.GetDepartmentIdByName(server, selectValue);
            string sql;
            List<Employee> employees = new List<Employee>();
            if (numSelectionValue == null)
                sql = $"SELECT id, firstname, secondname, patronymic, id_department, id_office, phone FROM Employee where {selectOption} Like '%{selectValue}%'";
            else
                sql = $"SELECT id, firstname, secondname, patronymic, id_department, id_office, phone FROM Employee where {selectOption} = '{numSelectionValue}'";

            if (selectOption == "" || selectValue == "")
                sql = "SELECT id, firstname, secondname, patronymic, id_department, id_office, phone FROM Employee";

            DataBase db = new DataBase(sql, server);

            if (db.data.HasRows)
            {
                while (db.data.Read())
                {
                    Employee emp = new Employee()
                    {
                        Id = db.data[0].ToString(),
                        Firstname = db.data[1].ToString(),
                        Secondname = db.data[2].ToString(),
                        Patronymic = db.data[3].ToString(),
                        id_Department = db.data[4].ToString(),
                        id_Cabinet = db.data[5].ToString(),
                        Phone = db.data[6].ToString()
                    };

                    employees.Add(emp);

                    emp.SetEmployeeDepartmentName(server);
                    emp.SetEmployeeCabinetsName(server);
                }
            }

            List<string> departmentsName = new List<string>();

            departmentsName = Department.GetAllDepartmentsName(server);

            ViewData["departmetList"] = departmentsName;
            ViewData["currentUser"] = user;

            db.Close();

            return View(employees);
        }


        [HttpGet]
        public IActionResult AuthorizationPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AuthorizationPage(string login, string pass, bool UnRegistred = false)
        {
            user = "";
            if (UnRegistred)
            {
                return RedirectToActionPermanent("Report", "Main");
            }
            User selectedUser = null;

            string sql = $"SELECT * FROM user  WHERE login = '{login.ToString()}' AND password = '{pass.ToString()}'";

            DataBase db = new DataBase(sql, server);

            if (db.data.HasRows)
            {
                while (db.data.Read())
                {
                    selectedUser = new User()
                    {
                        Id = db.data[0].ToString(),
                        Name = db.data[1].ToString(),
                        UserLogin = login,
                        UserPassword = pass
                    };
                }
            }
            if (selectedUser == null)
            {
                ViewData["Error Message"] = "Пользователь не найден";
                return View();
            }
            user = selectedUser.UserLogin;
            return RedirectToActionPermanent("Report", "Main");
        }
        public IActionResult AddNewRow(string selection)
        {
            switch (selection)
            {
                case "Department":
                    return RedirectToActionPermanent("AddDepartment", "Main");
                case "Employee":
                    return RedirectToActionPermanent("AddEmployee", "Main");
            }
            return View();
        }
        public IActionResult EditRow(string selection)
        {
            switch (selection)
            {
                case "Department":
                    return RedirectToActionPermanent("EditDepartment", "Main");
                case "Employee":
                    return RedirectToActionPermanent("EditEmployee", "Main");
            }
            return View();
        }
        public IActionResult DeleteRow(string selection)
        {
            switch (selection)
            {
                case "Department":
                    return RedirectToActionPermanent("DeleteDepartment", "Main");
                case "Employee":
                    return RedirectToActionPermanent("DeleteEmployee", "Main");
            }
            return View();
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            List<string> depList = Department.GetAllDepartmentsName(server);
            List<string> cabList = Cabines.GetAllCabinetName(server);
            ViewData["DepartmentList"] = depList;
            ViewData["CabinetList"] = cabList;
            return View();
        }

        [HttpPost]
        public IActionResult AddEmployee(string firstname, string secondname, string patronymic, string phone, string department, string cabinet)
        {
            StringBuilder errorMessage = new StringBuilder();
            int? departmentId = Department.GetDepartmentIdByName(server, department);
            int? cabinetId = Cabines.GetCabinetIdByName(server, cabinet);

            if (string.IsNullOrEmpty(firstname))
                errorMessage.AppendLine("Имя не заполненно");
            if (string.IsNullOrEmpty(secondname))
                errorMessage.AppendLine("Фамилия не заполненно");
            if (string.IsNullOrEmpty(patronymic))
                errorMessage.AppendLine("Отчетство не заполненно");
            if (string.IsNullOrEmpty(phone))
                errorMessage.AppendLine("Номер телефорна не заполнен");
            if (string.IsNullOrEmpty(department))
                errorMessage.AppendLine("Отдел не выбран");
            if (string.IsNullOrEmpty(cabinet))
                errorMessage.AppendLine("Кабинет не выбран");

            if (departmentId == null || cabinetId == null)
                errorMessage.AppendLine("Отдел или кабинет не найден");

            if (errorMessage.Length != 0)
            {
                List<string> depList = Department.GetAllDepartmentsName(server);
                List<string> cabList = Cabines.GetAllCabinetName(server);
                ViewData["DepartmentList"] = depList;
                ViewData["CabinetList"] = cabList;
                ViewData["Error Message"] = errorMessage.ToString();
                return View();
            }

            string sql = $"INSERT INTO employee (firstname,secondname,patronymic,id_department,id_department,phone)" +
                $"VALUES ('{firstname}','{secondname}','{patronymic}',{departmentId},{cabinetId},'{phone}')";

            DataBase db = new DataBase(sql, server);

            return RedirectToActionPermanent("Report", "Main");
        }

        [HttpGet]
        public IActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddDepartment(string name)
        {
            StringBuilder errorMessage = new StringBuilder();
            if (string.IsNullOrEmpty(name))
                errorMessage.AppendLine("Имя не заполненно");
            ViewData["Error Message"] = errorMessage.ToString();
            if (errorMessage.Length != 0)
                return View();

            string sql = $"INSERT INTO department (name)" +
                $"VALUES ('{name}')";

            DataBase db = new DataBase(sql, server);

            return RedirectToActionPermanent("Report", "Main");
        }

        [HttpGet]
        public IActionResult EditEmployee()
        {
            List<string> depList = Department.GetAllDepartmentsName(server);
            List<string> cabList = Cabines.GetAllCabinetName(server);
            ViewData["DepartmentList"] = depList;
            ViewData["CabinetList"] = cabList;
            List<string> empList = Employee.GetAllEmployeeName(server);
            ViewData["EmployeeList"] = empList;
            return View();
        }

        [HttpPost]
        public IActionResult EditEmployee(string firstname, string secondname, string patronymic, string phone, string department, string cabinet, string employee)
        {
            StringBuilder errorMessage = new StringBuilder();
            int? departmentId = Department.GetDepartmentIdByName(server, department);
            int? cabinetId = Cabines.GetCabinetIdByName(server, cabinet);

            if (string.IsNullOrEmpty(firstname))
                errorMessage.AppendLine("Имя не заполненно");
            if (string.IsNullOrEmpty(secondname))
                errorMessage.AppendLine("Фамилия не заполненно");
            if (string.IsNullOrEmpty(patronymic))
                errorMessage.AppendLine("Отчетство не заполненно");
            if (string.IsNullOrEmpty(phone))
                errorMessage.AppendLine("Номер телефорна не заполнен");
            if (string.IsNullOrEmpty(department))
                errorMessage.AppendLine("Отдел не выбран");
            if (string.IsNullOrEmpty(cabinet))
                errorMessage.AppendLine("Кабинет не выбран");

            if (departmentId == null || cabinetId == null)
                errorMessage.AppendLine("Отдел или кабинет не найден");

            if (errorMessage.Length != 0)
            {
                List<string> depList = Department.GetAllDepartmentsName(server);
                List<string> cabList = Cabines.GetAllCabinetName(server);
                ViewData["DepartmentList"] = depList;
                ViewData["CabinetList"] = cabList;
                List<string> empList = Employee.GetAllEmployeeName(server);
                ViewData["EmployeeList"] = empList;
                ViewData["Error Message"] = errorMessage.ToString();
                return View();
            }

            string employeId = employee.Split(' ')[1];

            string sql = $"Update Employee Set firstname = '{firstname}', secondname = '{secondname}', patronymic = '{patronymic}',id_department = {departmentId}, id_department = {cabinetId},phone = '{phone}'" +
                $"WHERE id = {employeId}";

            DataBase db = new DataBase(sql, server);

            return RedirectToActionPermanent("Report", "Main");
        }

        [HttpGet]
        public IActionResult EditDepartment()
        {
            List<string> depList = Department.GetAllDepartmentsName(server);
            ViewData["DepartmentList"] = depList;
            return View();
        }

        [HttpPost]
        public IActionResult EditDepartment(string name, string department)
        {
            StringBuilder errorMessage = new StringBuilder();
            int? departmentId = Department.GetDepartmentIdByName(server, department);
            if (string.IsNullOrEmpty(name))
                errorMessage.AppendLine("Имя не заполненно");
            ViewData["Error Message"] = errorMessage.ToString();
            if (errorMessage.Length != 0)
            {
                List<string> depList = Department.GetAllDepartmentsName(server);
                ViewData["DepartmentList"] = depList;
                return View();
            }

            string sql = $"Update department Set name = '{name}' where id = {departmentId}";

            DataBase db = new DataBase(sql, server);

            return RedirectToActionPermanent("Report", "Main");
        }

        [HttpGet]
        public IActionResult DeleteEmployee()
        {
            List<string> empList = Employee.GetAllEmployeeName(server);
            ViewData["EmployeeList"] = empList;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteEmployee(string employe)
        {

            string employeId = employe.Split(' ')[1];

            string sql = $"DELETE FROM Employee where id = {employeId}";

            DataBase db = new DataBase(sql, server);

            return RedirectToActionPermanent("Report", "Main");
        }

        [HttpGet]
        public IActionResult DeleteDepartment()
        {
            List<string> depList = Department.GetAllDepartmentsName(server);
            ViewData["DepartmentList"] = depList;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteDepartment(string department)
        {
            int? departmentId = Department.GetDepartmentIdByName(server, department);


            string sql = $"DELETE FROM employee where id_department = {departmentId}";


            DataBase db = new DataBase(sql, server);

            sql = $"DELETE FROM department where id = {departmentId}";

            db = new DataBase(sql, server);

            return RedirectToActionPermanent("Report", "Main");
        }

        [HttpGet]
        public IActionResult SelectEmployee()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelectEmployee(string selectOption, string selectValue)
        {
            return RedirectToAction("Report", "Main", new { selectOption = selectOption, selectValue = selectValue });
        }
    }
}
