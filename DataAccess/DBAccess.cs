﻿using System;
using Models;
using Models.Result;

namespace DataAccess
				connection.Open();

				var preSqlCommand = Parser.LazyParse<PreSqlCommand>(commandPath);
				SqlCommand command = preSqlCommand.ToSqlCommand();
				command.Connection = connection;

				var assembly = typeof(Order).Assembly;
				
				var type = assembly.GetType(preSqlCommand.Model);

				if (type is null)
				{
					type = assembly.GetType("Models." + preSqlCommand.Model);
				}

				MethodInfo execute = typeof(SqlCommandExtensions).GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
				execute = execute.MakeGenericMethod(type);


				var res = new Result<T>()
				{
					Table = execute.Invoke(null, new object[] { command }) as IEnumerable<T>,
					TypeOfTable = type
				};

				return res;
				throw ex;
			}
				connection.Close();
			}