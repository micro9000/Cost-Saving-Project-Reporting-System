using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IOperatorUserRepository : IRepository<OperatorUser>
	{
		OperatorUser Login (OperatorUser data);

		int ChangePassword (OperatorUser data);
	}
}
