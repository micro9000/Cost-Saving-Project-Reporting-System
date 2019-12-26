using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;

namespace Persistence.Repositories
{
	public class OperatorUserRepository : Repository<OperatorUser>, IOperatorUserRepository
	{
		public OperatorUser Login (OperatorUser data)
		{

			string sql = "SELECT * FROM Operators WHERE IsDeleted=0 AND FFID = @ffID AND Password=@password";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<OperatorUser>(sql, new
				{
					ffID = data.FFID,
					password = data.Password
				});

				conn.Close();

				return result;
			}
		}

		public int ChangePassword (OperatorUser data)
		{
			string query = "UPDATE Operators SET Password=@newPass WHERE FFID=@ffID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					ffID = data.FFID,
					newPass = data.Password
				});
				conn.Close();
			}

			return rowsUpdated;
		}
	}
}
