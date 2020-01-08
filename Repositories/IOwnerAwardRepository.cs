using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IOwnerAwardRepository : IRepository<OwnerAward>
	{
		List<OwnerAward> GetAllAwardsByUser (string userFFID);

		List<OwnerAward> GetAllAwardsByProject (int proposalID);
	}
}
