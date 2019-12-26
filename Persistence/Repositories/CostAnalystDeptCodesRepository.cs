using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class CostAnalystDeptCodesRepository : Repository<CostAnalystDeptCode>, ICostAnalystDeptCodesRepository
	{
		public List<CostAnalystDeptCode> GetDeptCodesByCostAnalystID (int costAnalystID)
		{
			var predicate = Predicates.Field<CostAnalystDeptCode>(c => c.CostAnalystID, Operator.Eq, costAnalystID);

			List<CostAnalystDeptCode> results = new List<CostAnalystDeptCode>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<CostAnalystDeptCode>(predicate).ToList();
				conn.Close();
			}
			return results;
		}

		public List<CostAnalystDeptCode> GetDepartments ()
		{
			string query = "SELECT DeptCode, DeptName FROM CostAnalystsDeptCodes WHERE isDeleted=0";

			List<CostAnalystDeptCode> results = new List<CostAnalystDeptCode>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<CostAnalystDeptCode>(query).ToList();

				conn.Close();

			}
			return results;
		}


		public CostAnalystDeptCode GetDepartment (string deptCodeOrName)
		{
			string query = "SELECT DeptCode, DeptName FROM CostAnalystsDeptCodes WHERE isDeleted=0 AND DeptName=@DeptName OR DeptCode=@DeptCode LIMIT 1";


			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<CostAnalystDeptCode>(query, new
				{
					DeptName = deptCodeOrName,
					DeptCode = deptCodeOrName
				});
				conn.Close();

				return result;

			}
		}

	}
}
