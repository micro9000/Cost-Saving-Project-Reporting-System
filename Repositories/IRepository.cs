using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Repositories
{
	public interface IRepository<TEntity> where TEntity : class
	{
		TEntity Get (int id);
		List<TEntity> GetAll ();
		List<TEntity> Find (Expression<Func<TEntity, bool>> predicate);

		// This method was not in the videos, but I thought it would be useful to add.
		TEntity SingleOrDefault (TEntity entity, string sql, bool isStoredProcedure = false);

		int Add (TEntity entity);
		int AddRange (IEnumerable<TEntity> entities, string sql, bool isStoredProcedure);


		bool Update (TEntity entity);

		int UpdateRange (IEnumerable<TEntity> entities, string sql, bool isStoredProcedure);

		bool Delete (TEntity entity);
		int DeleteRange (IEnumerable<TEntity> entities, string sql, bool isStoredProcedure);

	}
}
