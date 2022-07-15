//< add using >
using System.Data;
using System.Data.SqlClient;

namespace HoaBinhTools.SynchronizedData.Db
{
	public class DbConnect
	{
		public SqlConnection Get_DB_Connection()

		{
			SqlConnection cn_connection = new SqlConnection("Data Source=118.69.224.199,1444;Initial Catalog=BIMPORTAL_TEST2;Persist Security Info=True;User ID=taiht;Password=hbcg1235");

			if (cn_connection.State != ConnectionState.Open) cn_connection.Open();
			return cn_connection;
		}

		public DataTable Get_DataTable(string SQL_Text)

		{
			SqlConnection cn_connection = Get_DB_Connection();
			DataTable table = new DataTable();
			SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, cn_connection);

			adapter.Fill(table);
			return table;
		}

		public void Execute_SQL(string SQL_Text)

		{
			SqlConnection cn_connection = Get_DB_Connection();
			SqlCommand cmd_Command = new SqlCommand(SQL_Text, cn_connection);

			cmd_Command.ExecuteNonQuery();
		}

		public void Close_DB_Connection()

		{
			SqlConnection cn_connection = new SqlConnection("Data Source=118.69.224.199,1444;Initial Catalog=BIMPORTAL_TEST2;Persist Security Info=True;User ID=taiht;Password=hbcg1235");

			if (cn_connection.State != ConnectionState.Closed) cn_connection.Close();
		}
	}
}

