using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Dapper;
using System.Data;
using Repositories;
using Domain;
using DapperExtensions;


namespace Persistence.Repositories
{
	public class SOBRepository : Repository<SOBManagers>, ISOBRepository
	{
		public SOBRepository ()
		{
			//DapperExtensions.DapperExtensions.DefaultMapper = typeof(CustomPluralizedMapper<>);
			DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.MySqlDialect();
		}


		public override DbConnection GetOpenConnection ()
		{
			var connection = new MySqlConnection("Server=PHSM01WS012; Port=3306;Database=sob_db;Uid=automation;password=automation_APPs2017!;Persist Security Info=True;Allow Zero Datetime=True;");
			connection.Open();

			return connection;
		}



		public List<SOBManagers> SearchManagerByDeptCode (string deptCode)
		{
			var pga = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pga.Predicates.Add(Predicates.Field<SOBManagers>(p => p.deptCode, Operator.Eq, deptCode));
			pga.Predicates.Add(Predicates.Field<SOBManagers>(p => p.department_head, Operator.Eq, "Department Head"));

			List<SOBManagers> results = new List<SOBManagers>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<SOBManagers>(pga).ToList();
				conn.Close();
			}
			return results;
		}


		public SOBManagers GetManagerInfoByFFID (string ffID)
		{
			string sql = "SELECT * FROM masterlist WHERE ffId = @ffID ORDER BY id DESC LIMIT 1";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<SOBManagers>(sql, new
				{
					ffID = ffID
				});
				conn.Close();

				return result;
			}
		}


	}
}
