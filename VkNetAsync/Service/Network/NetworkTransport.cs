using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VkNetAsync.Service.Network
{
	public class NetworkTransport: INetworkTransport
	{
		private readonly HttpClient _client = new HttpClient(new HttpClientHandler() { UseCookies = true, CookieContainer = new CookieContainer()});

		public async Task<Response> Post(Uri uri, byte[] data = null, CancellationToken token = new CancellationToken())
		{
			HttpResponseMessage message = await _client.PostAsync(uri, new ByteArrayContent(data ?? new byte[0]), token);
			
			PrintResponseInfo(message);
			PrintContentInfo(message.Content);
			
			return new Response(message.RequestMessage.RequestUri, await message.Content.ReadAsStringAsync());
		}

		public async Task<Response> Get(Uri uri, CancellationToken token = new CancellationToken())
		{
			var message = await _client.GetAsync(uri, token);
			
			PrintResponseInfo(message);
			PrintContentInfo(message.Content);

			return new Response(message.RequestMessage.RequestUri, await message.Content.ReadAsStringAsync());
		}

		[Conditional("DEBUG")]
		private static void PrintResponseInfo(HttpResponseMessage response)
		{
			Debug.WriteLine("******************* RESPONSE *******************************");
			Debug.WriteLine("time: {0:hh:mm:ss:fff}", DateTime.Now);
			Debug.WriteLine("status code: " + response.StatusCode);
			Debug.WriteLine("request message: " + response.RequestMessage);
			Debug.WriteLine("------------- response headers: ------------");
			foreach (var header in response.Headers)
				Debug.WriteLine(header.Key + ": " + string.Join(", ", header.Value) + "\n");
			Debug.WriteLine("***************** END RESPONSE *****************************\n\n");
		}

		[Conditional("DEBUG")]
		private static async void PrintContentInfo(HttpContent content)
		{
			Debug.WriteLine("******************* CONTENT ********************************");
			Debug.WriteLine("time: {0:hh:mm:ss:fff}", DateTime.Now);
			Debug.WriteLine("------------- content headers: ------------");
			foreach (var header in content.Headers)
				Debug.WriteLine(header.Key + ": " + string.Join(", ", header.Value) + "\n");
			Debug.WriteLine("------------------ content ----------------");
			Debug.WriteLine(await content.ReadAsStringAsync());
			Debug.WriteLine("***************** END CONTENT ******************************\n\n");
		}

		
	}
}