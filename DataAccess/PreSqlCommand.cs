using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
	public class PreSqlCommand
	{
		public string CommandText { get; set; }
		public List<PreSqlParam> Params { get; set; } = null;
		public int CommandTimeout { get; set; } = 30;
		public short CommandType { get; set; }
		public string TypeOfTable { get; set; }
		public string Model { get; set; }
		public SqlCommand ToSqlCommand()
		{
			var command = new SqlCommand();
			command.CommandText = CommandText;
			command.CommandTimeout = CommandTimeout;
			command.CommandType = (System.Data.CommandType)(int)CommandType;

			if (Params != null)
			{
				foreach (var param in Params)
				{
					command.Parameters.AddWithValue(param.ParameterName, param.Value);
				}
			}

			return command;
		}
	}

	public class PreSqlParam
	{
		public string ParameterName { get; set; }
		public string Value { get; set; }
	}
}
