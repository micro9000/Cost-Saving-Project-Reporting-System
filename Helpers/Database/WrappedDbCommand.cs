using System;
using System.Data;

namespace Helpers.Database
{
	public class WrappedDbCommand : IDbCommand
	{
		private readonly IDbCommand _cmd;
		public WrappedDbCommand (IDbCommand command)
		{
			if (command == null)
				throw new ArgumentNullException(CSharpFiveExtension.nameof(() => command)); // nameof(command)

			_cmd = command;
		}


		public string CommandText
		{
			get
			{
				return _cmd.CommandText;
			}
			set
			{
				_cmd.CommandText = value;
			}
		}

		public int CommandTimeout
		{
			get
			{
				return _cmd.CommandTimeout;
			}
			set
			{
				_cmd.CommandTimeout = value;
			}
		}

		public CommandType CommandType
		{
			get
			{
				return _cmd.CommandType;
			}
			set
			{
				_cmd.CommandType = value;
			}
		}

		public IDbConnection Connection
		{
			get
			{
				return _cmd.Connection;
			}
			set
			{
				_cmd.Connection = value;
			}
		}

		public IDataParameterCollection Parameters
		{
			get
			{
				return _cmd.Parameters;
			}
		}

		public IDbTransaction Transaction
		{
			get
			{
				return _cmd.Transaction;
			}
			set
			{
				_cmd.Transaction = value;
			}
		}

		public UpdateRowSource UpdatedRowSource
		{
			get
			{
				return _cmd.UpdatedRowSource;
			}
			set
			{
				_cmd.UpdatedRowSource = value;
			}
		}

		public void Cancel ()
		{
			try
			{

				_cmd.Cancel();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[Cancel "+ ex.Message +"]");
			}
		}

		public IDbDataParameter CreateParameter ()
		{
			try
			{

				return _cmd.CreateParameter();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[CreateParameter "+ ex.Message +"]");
			}
		}

		public void Dispose ()
		{
			try
			{

				_cmd.Dispose();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[Dispose ["+ ex.Message +"]");
			}

		}

		public int ExecuteNonQuery ()
		{
			try
			{
				//Console.WriteLine($"[ExecuteNonQuery] {_cmd.CommandText}");
				return _cmd.ExecuteNonQuery();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[ExecuteNonQuery "+ _cmd.CommandText +" ---> " + ex.Message);
			}
		}

		public IDataReader ExecuteReader ()
		{
			try
			{
				//Console.WriteLine($"[ExecuteReader] {_cmd.CommandText}");
				return _cmd.ExecuteReader();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[ExecuteReader "+ _cmd.CommandText +" ---> " +ex.Message);
			}

		}

		public IDataReader ExecuteReader (CommandBehavior behavior)
		{
			try
			{
				return _cmd.ExecuteReader();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[ExecuteReader("+ behavior +")] "+ _cmd.CommandText +" ---> " + ex.Message);
			}
			//Console.WriteLine($"[ExecuteReader({behavior})] {_cmd.CommandText}");

		}

		public object ExecuteScalar ()
		{
			try
			{
				//Console.WriteLine($"[ExecuteScalar] {_cmd.CommandText}");
				return _cmd.ExecuteScalar();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[ExecuteScalar "+ _cmd.CommandText +" ---> " + ex.Message);
			}


		}

		public void Prepare ()
		{
			try
			{

				_cmd.Prepare();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				throw new Exception("[Prepare " + ex.Message);
			}
		}
	}
}
