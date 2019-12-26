using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class FinanceCategoryRepository : Repository<FinanceCategory>, IFinanceCategory
	{
		public List<FinanceCategory> GetAllCategories ()
		{
			var results = new List<FinanceCategory>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<FinanceCategory>().ToList();
			}

			return results;
		}
	}
}
