using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
	public class CustomPluralizedMapper<T> : DapperExtensions.Mapper.PluralizedAutoClassMapper<T> where T : class
	{
		public override void Table (string tableName)
		{
			if (tableName.Equals("Proposal", StringComparison.CurrentCultureIgnoreCase))
			{
				TableName = "Proposals";
			}

			if (tableName.Equals("OperatorUser", StringComparison.CurrentCultureIgnoreCase))
			{
				TableName = "Operators";
			}

			base.Table(tableName);
		}


	}
}
