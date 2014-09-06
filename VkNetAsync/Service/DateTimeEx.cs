using System;
using System.Diagnostics.Contracts;

namespace VkNetAsync.Service
{
	public static class DateTimeEx
	{
		public static long ToUnixTime(this DateTime timestamp)
		{
			Contract.Requires<ArgumentOutOfRangeException>(timestamp >= new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			Contract.Ensures(Contract.Result<long>() >= 0);
			
			return (long) (timestamp.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		}
		
		public static DateTime UnixTimeToDateTime(long unixtime)
		{
			Contract.Requires<ArgumentOutOfRangeException>(unixtime >= 0);

			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixtime);
		}

	}
}