using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace VkNetAsync.API
{
	public sealed class VkApi
	{
		public long UserId { get; private set; }

		public string AccessToken { get; private set; }

		public VkApi(long userId, string accessToken)
		{
			Contract.Requires<ArgumentOutOfRangeException>(userId > 0);
			Contract.Requires<ArgumentNullException>(accessToken != null);
			Contract.Requires<FormatException>(Regex.IsMatch(accessToken, @"[0-9a-f]+"));
			
			UserId = userId;
			AccessToken = accessToken;
		}

		

		

	}
}