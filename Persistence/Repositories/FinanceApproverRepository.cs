using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class FinanceApproverRepository : Repository<FinanceApprover>, IFinanceApproverRepository
	{
		public FinanceApprover GetInfoByFFID (string ffID)
		{
			string query = "SELECT * FROM FinanceApprovers WHERE IsDeleted=0 AND FFID = @ffID ORDER BY id DESC LIMIT 1";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<FinanceApprover>(query, new
				{
					ffID = ffID
				});
				conn.Close();

				return result;
			}
		}




	}
}
