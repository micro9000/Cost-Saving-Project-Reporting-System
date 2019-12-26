using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IValidatorRepository : IRepository<Validator>
	{
		Validator GetValidatorByFFID (string ffID);

		IEnumerable<Validator> GetValidatorByType (StaticData.UserTypes userType);
	}
}
