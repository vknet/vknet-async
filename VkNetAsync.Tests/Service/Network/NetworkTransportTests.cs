using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using VkNetAsync.Service.Network;

namespace VkNetAsync.Tests.Service.Network
{
	[TestFixture]
	public class NetworkTransportTests
	{
		[Test]
		public async void Post_HaveParameters()
		{
			var networkTransport = new NetworkTransport();

			var json = Newtonsoft.Json.Linq.JObject.Parse((await networkTransport.Post(new Uri(@"https://api.vk.com/method/users.get"), Encoding.UTF8.GetBytes(@"user_ids=205387401"))).ResponseData);
			
			Assert.That(json.ToString(Formatting.None).Replace('"', '\''), Is.EqualTo(@"{'response':[{'uid':205387401,'first_name':'Tom','last_name':'Cruise'}]}"));
		}

		[Test]
		public async void Post_HaveNoParameters()
		{
			var networkTransport = new NetworkTransport();

			var json = Newtonsoft.Json.Linq.JObject.Parse((await networkTransport.Post(new Uri(@"https://api.vk.com/method/utils.getServerTime"))).ResponseData);
			
			Assert.That((long)json["response"], Is.GreaterThan(0));
		}

		[Test]
		public void Post_Cancel_ThrowsTaskCancelledException()
		{
			var networkTransport = new NetworkTransport();

			var source = new CancellationTokenSource();
			source.Cancel();
			
			Assert.Throws<TaskCanceledException>(async () => await networkTransport.Post(new Uri(@"https://api.vk.com/method/utils.getServerTime"), token: source.Token));
			
		}
	}
}