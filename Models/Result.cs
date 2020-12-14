using System;
using System.Collections.Generic;

namespace Models.Result
{
	public class Result<T> where T: new()
	{
		public IEnumerable<T> Table { get; set; }
		public Type TypeOfTable { get; set; }
	}
}
