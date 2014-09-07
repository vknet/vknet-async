using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VkNetAsync.Service.Network
{
	public class NetworkTransport: INetworkTransport
	{
		public async Task<string> Post(Uri uri, CancellationToken token = new CancellationToken())
		{
			var requestUri = uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Host | UriComponents.Path, UriFormat.UriEscaped);

			WebRequest request = WebRequest.Create(requestUri);
			
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			var postData = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped);
			byte[] byteArray = Encoding.UTF8.GetBytes(postData);

			using (Stream dataStream = await Task.Factory.FromAsync(request.BeginGetRequestStream, result => request.EndGetRequestStream(result), null))
				await dataStream.WriteAsync(byteArray, 0, byteArray.Length, token);

			using (WebResponse response = await request.GetResponseAsync())
			using (var dataStream = response.GetResponseStream() ?? Stream.Null)
			using (StreamReader reader = new StreamReader(dataStream))
			{
				return await reader.ReadToEndAsync();
			}
		}
	}
}