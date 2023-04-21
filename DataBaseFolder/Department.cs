using Microsoft.AspNetCore.Hosting.Server;
using System.Numerics;

namespace HandBook.DataBaseFolder
{
	public class Department
	{
		public List<Department> ListModel = new List<Department>();

		public string Id { get; set; }
		public string Name { get; set; }

		public static List<string> GetAllDepartmentsName(IConfiguration server)
		{
			List<string> allDepartments = new List<string>();
            string sql = $"SELECT Name FROM Department Order by Name";

            DataBase db = new DataBase(sql, server);

            if (db.data.HasRows)
			{
				while(db.data.Read())
				{
					allDepartments.Add(db.data[0].ToString());
				}
			}

			return allDepartments;
        }

        public static int? GetDepartmentIdByName(IConfiguration server, string name)
        {
            string sql = $"SELECT id FROM Department Where name = '{name}'";
            int? departmentId = null;

            DataBase db = new DataBase(sql, server);

            if (db.data.HasRows)
            {
                while (db.data.Read())
                {
                    departmentId = int.Parse(db.data[0].ToString());
                }
            }

            return departmentId;
        }
    }
}
