using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class TrackingCategoryRepository : Repository<TrackingCategory>, ITrackingCategoryRepository
	{
	}
}
