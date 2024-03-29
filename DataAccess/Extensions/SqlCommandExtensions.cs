﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Extensions
{
	//with help from Artsiom
	public static class SqlCommandExtensions
	{
		public static IEnumerable<TEntity> Execute<TEntity>(this SqlCommand command) where TEntity : new()
		{
			var result = ExecuteInternal<TEntity>(command);
			return result;
		}

		private static IEnumerable<TEntity> ExecuteInternal<TEntity>(SqlCommand command) where TEntity : new()
		{
			var reader = command.ExecuteReader();

			if (!reader.HasRows)
			{
				reader.Close();
				return Enumerable.Empty<TEntity>();
			}

			var entities = reader.ParseFromReaderInternal<TEntity>();
			reader.Close();

			return entities;
		}

		private static IEnumerable<TEntity> ParseFromReaderInternal<TEntity>(this SqlDataReader reader) where TEntity : new()
		{
			var entityType = typeof(TEntity);
			var entityProps = entityType.GetProperties();

			var entities = new List<TEntity>();

			TEntity entity;
			object valueFromReader;

			while (reader.Read())
			{
				entity = new TEntity();
				foreach (var entityPropInfo in entityProps)
				{
					try
					{
						valueFromReader = reader[entityPropInfo.Name];
						if (valueFromReader is DBNull)
						{
							valueFromReader = null;
							continue;
						}

						entityPropInfo.SetValue(entity, valueFromReader);
					}
					catch (IndexOutOfRangeException)
					{
						continue;
					}
				}

				entities.Add(entity);
			}
			return entities;
		}
	}
}
