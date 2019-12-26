using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;

namespace Persistence.Repositories
{
	public class ValidatorRepository : Repository<Validator>, IValidatorRepository
	{
		public Validator GetValidatorByFFID (string ffID)
		{

			string sql = "SELECT * FROM Validator WHERE FFID = @ffID ORDER BY id DESC LIMIT 1";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<Validator>(sql, new
				{
					ffID = ffID
				});
				conn.Close();

				return result;
			}

		}

		public IEnumerable<Validator> GetValidatorByType (StaticData.UserTypes userType)
		{
			var predicate = Predicates.Field<Validator>(v => v.ValidatorType, Operator.Eq, userType.ToString());

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var list = conn.GetList<Validator>(predicate);
				conn.Close();
				return list;
			}
		}
	}
}
