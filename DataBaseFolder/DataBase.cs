using Npgsql;

namespace HandBook.DataBaseFolder
{
	public class DataBase
	{
		public Server server;
		string sql;
		public NpgsqlDataReader data;
		public DataBase(string sql, IConfiguration server)
		{
			this.sql = sql;
			this.server = new Server(server);
			NpgsqlCommand query = new NpgsqlCommand(sql, this.server.Connection);
			data = query.ExecuteReader();
		}

		public void Close()
		{
			this.server.Connection.Close();
		}
	}
	public class Server
	{
		public NpgsqlConnection Connection;
		public Server(IConfiguration server)
		{
			string connectionString = String.Format(server.GetSection("MyServer").Value);
			
			Connection = new NpgsqlConnection(connectionString);

			Connection.Open();
		}
	}
	
}
