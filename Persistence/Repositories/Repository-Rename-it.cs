using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Dapper;
using System.Data;
using Repositories;

namespace Persistence.Repositories
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		public Repository ()
		{
			//DapperExtensions.DapperExtensions.DefaultMapper = typeof(CustomPluralizedMapper<>);
			DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.MySqlDialect();
		}

		public virtual DbConnection GetOpenConnection ()
		{
			// Localhost
			var connection = new MySqlConnection("Server=127.0.0.1; Port=3306;Database=E_cost_saving_db;Uid=root;password=;Persist Security Info=True;Allow Zero Datetime=True;");

			connection.Open();

			return connection;
		}


		//public static DbConnection GetSOBOpenConnection ()
		//{
		//	var connection = new MySqlConnection("Server=PHSM01WS012; Port=3306;Database=sob_db;Uid=automation;password=automation_APPs2017!;Persist Security Info=True;Allow Zero Datetime=True;");
		//	connection.Open();

		//	return connection;
		//}


		public int Add (TEntity entity)
		{
			var id = 0;
			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				id = DapperExtensions.DapperExtensions.Insert<TEntity>(conn, entity);

				//id = (int)Dapper.Contrib.Extensions.SqlMapperExtensions.Insert<TEntity>(conn, entity);
				conn.Close();

			}
			return id;
		}


		public int AddRange (IEnumerable<TEntity> entities, string sql, bool isStoredProcedure = false)
		{

			var affectedRows = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{

				if (isStoredProcedure == true)
				{
					affectedRows = (int)conn.Execute(sql, entities, commandType: CommandType.StoredProcedure);
				}
				else
				{
					affectedRows = (int)conn.Execute(sql, entities);
				}

				conn.Close();
			}

			return affectedRows;

		}



		public bool Update (TEntity entity)
		{
			var isSuccess = false;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				isSuccess = Dapper.Contrib.Extensions.SqlMapperExtensions.Update<TEntity>(conn, entity);
				conn.Close();
			}

			return isSuccess;
		}

		public int UpdateRange (IEnumerable<TEntity> entities, string sql, bool isStoredProcedure = false)
		{
			var affectedRows = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{

				if (isStoredProcedure == true)
				{
					affectedRows = (int)conn.Execute(sql, entities, commandType: CommandType.StoredProcedure);
				}
				else
				{
					affectedRows = (int)conn.Execute(sql, entities);
				}
				conn.Close();
			}

			return affectedRows;
		}


		public bool Delete (TEntity entity)
		{
			var isSuccess = false;
			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{

				isSuccess = Dapper.Contrib.Extensions.SqlMapperExtensions.Delete<TEntity>(conn, entity);
				conn.Close();
			}
			return isSuccess;
		}


		public int DeleteRange (IEnumerable<TEntity> entities, string sql, bool isStoredProcedure = false)
		{
			var affectedRows = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{

				if (isStoredProcedure == true)
				{
					affectedRows = (int)conn.Execute(sql, entities, commandType: CommandType.StoredProcedure);
				}
				else
				{
					affectedRows = (int)conn.Execute(sql, entities);
				}

				conn.Close();
			}

			return affectedRows;
		}


		public virtual List<TEntity> Find (Expression<Func<TEntity, bool>> predicate)
		{
			throw new NotImplementedException();
		}

		public TEntity Get (int id)
		{
			TEntity results;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = Dapper.Contrib.Extensions.SqlMapperExtensions.Get<TEntity>(conn, id);

				conn.Close();
			}

			return results;
		}

		public List<TEntity> GetAll ()
		{
			List<TEntity> results = new List<TEntity>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = Dapper.Contrib.Extensions.SqlMapperExtensions.GetAll<TEntity>(conn).ToList();
				conn.Close();

			}


			return results;
		}

		public TEntity SingleOrDefault (TEntity entity, string sql, bool isStoredProcedure = false)
		{
			TEntity results;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{


				if (isStoredProcedure == true)
				{
					results = conn.QuerySingleOrDefault<TEntity>(sql, entity, commandType: CommandType.StoredProcedure);
				}
				else
				{
					results = conn.QuerySingleOrDefault<TEntity>(sql, entity);
				}


				conn.Close();

			}


			return results;
		}

	}
}
