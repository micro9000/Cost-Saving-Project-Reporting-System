using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class CostAnalystRepository : Repository<CostAnalyst>, ICostAnalystRepository
	{
		public CostAnalyst GetInfoByFFID (string ffID)
		{
			string query = "SELECT * FROM CostAnalysts WHERE IsDeleted=0 AND FFID = @ffID ORDER BY id DESC LIMIT 1";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<CostAnalyst>(query, new
				{
					ffID = ffID
				});
				conn.Close();

				return result;
			}
		}

		public List<CostAnalyst> GetInfoByDeptCode (string deptCode)
		{
			//			string query = @"SELECT CA.*, CAD.*
			//							FROM CostAnalysts AS CA
			//							JOIN CostAnalystsDeptCodes AS CAD ON CA.id=CAD.CostAnalystID
			//							WHERE CA.isDeleted=0 AND CAD.isDeleted=0 AND CAD.DeptCode=@DeptCode";

			string query = @"SELECT CA.*, CAD.*
							FROM CostAnalysts AS CA
							JOIN CostAnalystsDeptCodes AS CAD ON CA.id=CAD.CostAnalystID
							WHERE CA.isDeleted=0 AND CAD.isDeleted=0 AND (CAD.DeptName=@DeptCode OR CAD.DeptCode=@DeptCode) LIMIT 1";

			var p = new
			{
				DeptCode = deptCode
			};


			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.Query<CostAnalyst, CostAnalystDeptCode, CostAnalyst>(query,
					(CA, CAD) =>
					{
						CA.DeptCodes = new List<CostAnalystDeptCode>();
						CA.DeptCodes.Add(CAD);
						return CA;
					}, p).ToList();
				conn.Close();

				return result;
			}
		}


		public List<CostAnalyst> GetAllCostAnalyst ()
		{
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};

			pgMain.Predicates.Add(Predicates.Field<CostAnalyst>(f => f.IsDeleted, Operator.Eq, 0));


			List<CostAnalyst> results = new List<CostAnalyst>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<CostAnalyst>(pgMain).ToList();

				conn.Close();
			}

			return results;
		}

	}
}
