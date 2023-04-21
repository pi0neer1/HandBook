using System.Numerics;

namespace HandBook.DataBaseFolder
{
	public class Employee
	{
		public string Id { get; set; }
		public string Firstname { get; set; }
		public string Secondname { get; set; }
		public string Patronymic { get; set; }
		public string Phone { get; set; }
		public string id_Department { get; set; }
		public string id_Cabinet { get; set; }

		public virtual Department Department { get; set; }
		public virtual Cabines Cabinets { get; set; }

		public void SetEmployeeDepartmentName(IConfiguration server)
		{
			string sql = $"SELECT Name FROM Department WHERE Id = {id_Department}";

			DataBase db = new DataBase(sql, server);

			this.Department = new Department();

			if (!db.data.HasRows)
				return;
			Department.Id = id_Department;
			while(db.data.Read())
				Department.Name = db.data[0].ToString();
		}

		public void SetEmployeeCabinetsName(IConfiguration server)
		{
			string sql = $"SELECT Name FROM office WHERE Id = {id_Cabinet}";

			DataBase db = new DataBase(sql, server);

			this.Cabinets = new Cabines();

			if (!db.data.HasRows)
				return;
			Cabinets.Id = id_Cabinet;
            while (db.data.Read())
                Cabinets.Name = db.data[0].ToString();
			return;

		}

        public static List<string> GetAllEmployeeName(IConfiguration server)
        {
            List<string> allDepartments = new List<string>();
			string sql = $" SELECT Employee.id,\r\n    Employee.firstname,\r\n    Employee.secondname,\r\n    Employee.patronymic,\r\n    department.name AS departmentname,\r\n    office.name AS cabinetname,\r\n    Employee.phone\r\n   FROM Employee\r\n     JOIN department ON Employee.id_department = department.id\r\n     JOIN office ON Employee.id_cabinet = office.id;";

			DataBase db = new DataBase(sql, server);

            if (db.data.HasRows)
            {
                while (db.data.Read())
                {
                    allDepartments.Add("id" + " " + db.data[0].ToString() + " " + db.data[1].ToString() + " " + db.data[2].ToString() + " " + db.data[3].ToString() + " Отдел: " + db.data[4].ToString() + " Кабинет: " + db.data[5].ToString() + " Телефон: " + db.data[6].ToString());
                }
            }
            return allDepartments;
        }
    }
}
