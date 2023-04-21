using System.Numerics;

namespace HandBook.DataBaseFolder
{
	public class Cabines
	{
		public List<Cabines> ListModel = new List<Cabines>();

		public string Id { get; set; }
		public string Name { get; set; }

        public static List<string> GetAllCabinetName(IConfiguration server)
        {
            List<string> allCabinet = new List<string>();
            string sql = $"SELECT Name FROM office Order by Name";

            DataBase db = new DataBase(sql, server);

            if (db.data.HasRows)
            {
                while (db.data.Read())
                {
                    allCabinet.Add(db.data[0].ToString());
                }
            }

            return allCabinet;
        }

        public static int? GetCabinetIdByName(IConfiguration server, string name)
        {
            string sql = $"SELECT id FROM office Where name = '{name}'";
            int? cabinetId = null;

            DataBase db = new DataBase(sql, server);

            if (db.data.HasRows)
            {
                while (db.data.Read())
                {
                    cabinetId = int.Parse(db.data[0].ToString());
                }
            }

            return cabinetId;
        }
    }
}
