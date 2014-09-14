using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using VkNetAsync.Annotations;
using VkNetAsync.Service;
using VkNetAsync.Service.Exception;
using VkNetAsync.Service.Network;

namespace VkNetAsync.API
{
	public sealed class VkApi
	{
		private readonly AsyncLock _callLock = new AsyncLock();
		private readonly TimeSpan _requestsTimeout = TimeSpan.FromSeconds(0.34);
		private DateTime? _lastCallTime;

		private const string VkUrl = @"https://api.vk.com/method/";

		private readonly INetworkTransport _transport;

		public long UserId { get; private set; }

		public string AccessToken { get; private set; }


		internal VkApi(long userId, string accessToken, [NotNull] INetworkTransport transport)
		{
			Contract.Requires<ArgumentNullException>(transport != null);
			Contract.Requires<ArgumentOutOfRangeException>(userId > 0);
			Contract.Requires<ArgumentNullException>(accessToken != null);
			Contract.Requires<FormatException>(Regex.IsMatch(accessToken, @"^[0-9a-f]+$"));

			_transport = transport;

			UserId = userId;
			AccessToken = accessToken;
		}


		public async Task<JToken> Call([NotNull] string method, [CanBeNull] VkParameters parameters = null, CancellationToken token = default(CancellationToken))
		{
			Contract.Requires<ArgumentNullException>(method != null);
			Contract.Requires<ArgumentException>(method.Length > 0);

			(parameters ?? (parameters = new VkParameters())).Add("access_token", AccessToken);

			var uri = new Uri(string.Format("{0}{1}", VkUrl, method));
			byte[] parametersData = Encoding.UTF8.GetBytes(parameters.ToString());

			var response = JObject.Parse(await Call(uri, parametersData, token));

			if (response["error"] != null)
				throw VkErrorConverter.Convert((JObject)response["error"]);

			return response["response"];
		}

		private async Task<string> Call(Uri uri, byte[] data, CancellationToken token)
		{
			using (await _callLock.LockAsync(token))
			{
				try
				{
					if (_lastCallTime != null)
					{
						var delay = (_lastCallTime.Value - DateTime.UtcNow) + _requestsTimeout;
						if (delay > TimeSpan.Zero)
							await Task.Delay(delay, token);
					}
				}
				finally
				{
					_lastCallTime = DateTime.UtcNow;
				}

				return (await _transport.Post(uri, data, token)).ResponseData;
			}
		}
	}
}