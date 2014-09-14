using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Nito.AsyncEx;
using NUnit.Framework;
using VkNetAsync.API;
using VkNetAsync.Service;
using VkNetAsync.Service.Exception;
using VkNetAsync.Service.Network;

namespace VkNetAsync.Tests.API
{
	[TestFixture]
	public class VkApiTests
	{
		#region Constructor

		[Test]
		[TestCase(1, "a"), TestCase(10, "1"), TestCase(long.MaxValue, "a345e44cb39c4f")]
		public void Constructor_CorrectParameters_DoesNotThrow(long userId, string token)
		{
			Assert.DoesNotThrow(() => new VkApi(userId, token, Mock.Of<INetworkTransport>()));
		}

		[Test]
		[TestCase(0), TestCase(-1), TestCase(-10), TestCase(long.MinValue)]
		public void Constructor_IncorrectUserId_ThrowsArgumentOutOfRange(long userId)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new VkApi(userId, "abc", Mock.Of<INetworkTransport>()));
		}

		[Test]
		public void Constructor_NullToken_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new VkApi(1, null, Mock.Of<INetworkTransport>()));
		}

		[Test]
		[TestCase(""), TestCase("g"), TestCase("not!a%token"), TestCase("совсемНеТокен")]
		public void Constructor_IncorrectToken_ThrowsFormatException(string token)
		{
			Assert.Throws<FormatException>(() => new VkApi(1, token, Mock.Of<INetworkTransport>()));
		}

		[Test]
		public void Constructor_NullNetworkTransport_ThrowsArgumentNullException()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Assert.Throws<ArgumentNullException>(() => new VkApi(1, "a", null));
		}

		#endregion


		#region Call

		[Test]
		public void Call_IncorrectMethod_ThrowsArgumentNullException()
		{
			var api = new VkApi(1, "abc", new Mock<INetworkTransport>(MockBehavior.Strict).Object);
			// ReSharper disable once AssignNullToNotNullAttribute
			Assert.Throws<ArgumentNullException>(async () => await api.Call(null));
			Assert.Throws<ArgumentException>(async () => await api.Call(string.Empty));
		}

		[Test]
		public async void Call_ServerReturnsResponse_ReturnsResponseContent()
		{
			const string response = @"{'response':{'val':2}}";
			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), CancellationToken.None)).Returns<Uri, byte[], CancellationToken>((uri, bytes, token) => Task.FromResult(new Response(uri, response)));
			var api = new VkApi(1, "abc", mock.Object);

			Assert.That((await api.Call("method.name")).ToString(Formatting.None), Is.EqualTo("{\"val\":2}"));
		}

		[Test]
		public void Call_ServerReturnsError_ThrowsException()
		{
			const string response = @"{'error':{'error_code': 113,'error_msg':'Invalid user id'}}";
			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).Returns<Uri, byte[], CancellationToken>((uri, bytes, token) => Task.FromResult(new Response(uri, response)));
			var api = new VkApi(1, "abc", mock.Object);

			Assert.That(async () => await api.Call("method.name"), Throws.InstanceOf<VkException>().With.Property("ErrorCode").EqualTo(113));
		}

		[Test]
		public async void Call_NotDefaultToken_PassedToNetworkTransport()
		{
			const string response = @"{'response':{}}";
			var tokenSource = new CancellationTokenSource();

			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsNotNull<byte[]>(), tokenSource.Token)).Returns<Uri, byte[], CancellationToken>((uri, bytes, token) => Task.FromResult(new Response(uri, response)));
			var api = new VkApi(1, "abc", mock.Object);

			await api.Call("method.name", token: tokenSource.Token);
			var data = Encoding.UTF8.GetBytes("access_token=abc");
			mock.Verify(t => t.Post(new Uri(@"https://api.vk.com/method/method.name"), data, tokenSource.Token), Times.Once);
		}

		[Test]
		public void Call_Cancel_CancelsCallExecution()
		{
			const string response = @"{'response':{}}";
			var tokenSource = new CancellationTokenSource();

			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), tokenSource.Token))
				.Returns<Uri, byte[],  CancellationToken>(async (uri, array, token) =>
													   {
														   await Task.Delay(TimeSpan.FromSeconds(2), token);
														   Assert.Fail("Task was not cancelled.");
														   return new Response(new Uri(""), response);
													   });
			var api = new VkApi(1, "abc", mock.Object);
			tokenSource.CancelAfter(TimeSpan.FromSeconds(0.1));
			
			Assert.Throws<TaskCanceledException>(async () => await api.Call("method.name", null, tokenSource.Token));
			mock.Verify(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), tokenSource.Token), Times.Once);
		}

		[Test]
		public async void Call_NullParameters_AddsTokenToParameters()
		{
			const string response = @"{'response':{'val':2}}";
			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), CancellationToken.None)).Returns<Uri, byte[], CancellationToken>((uri, bytes, token) => Task.FromResult(new Response(uri, response)));
			var api = new VkApi(1, "abc", mock.Object);

			Assert.That((await api.Call("method.name")).ToString(Formatting.None), Is.EqualTo("{\"val\":2}"));
			var data = Encoding.UTF8.GetBytes("access_token=abc");
			mock.Verify(t => t.Post(new Uri(@"https://api.vk.com/method/method.name"), data, CancellationToken.None), Times.Once);
		}

		[Test]
		public async void Call_NotNullParameters_AddsTokenToParameters()
		{
			const string response = @"{'response':{'val':2}}";
			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), CancellationToken.None)).Returns<Uri, byte[], CancellationToken>((uri, bytes, token) => Task.FromResult(new Response(uri, response)));
			var api = new VkApi(1, "abc", mock.Object);

			Assert.That((await api.Call("method.name", new VkParameters(){{"parameter", "value"}})).ToString(Formatting.None), Is.EqualTo("{\"val\":2}"));
			var data = Encoding.UTF8.GetBytes("parameter=value&access_token=abc");
			mock.Verify(t => t.Post(new Uri(@"https://api.vk.com/method/method.name"), data, CancellationToken.None), Times.Once);
		}

		[Test]
		[Timeout(5000)]
		public void Call_Concurrent()
		{
			const string response = @"{'response':{'val':2}}";
			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), CancellationToken.None)).Returns<Uri, byte[], CancellationToken>((uri, bytes, token) => Task.FromResult(new Response(uri, response)));
			var api = new VkApi(1, "abc", mock.Object);

			var stopwatch = Stopwatch.StartNew();
			Func<Task> callLoop = async () =>
										{
											for (int i = 0; i < 4; i++)
											{
												await api.Call("method.name");
												Console.WriteLine("i = {0} at task {1:00}, elapsed {2}", i, Thread.CurrentThread.ManagedThreadId, stopwatch.Elapsed);
											}
										};
			Task.WaitAll(Task.Run(callLoop),
						 Task.Run(callLoop),
						 Task.Run(callLoop));
			stopwatch.Stop();

			Assert.That(Math.Abs(stopwatch.Elapsed.TotalSeconds - (12 * (1f / 3f))) < 1);

			mock.Verify(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), CancellationToken.None), Times.Exactly(12));
		}

		[Test]
		[Timeout(5000)]
		public void Call_CancelConcurrent()
		{
			const string response = @"{'response':{'val':2}}";
			var tokenSource = new CancellationTokenSource();
			
			var mock = new Mock<INetworkTransport>(MockBehavior.Strict);
			mock.Setup(t => t.Post(It.IsAny<Uri>(), It.IsAny<byte[]>(), tokenSource.Token)).Returns<Uri, byte[], CancellationToken>((uri, bytes, token) => Task.FromResult(new Response(uri, response)));
			var api = new VkApi(1, "abc", mock.Object);


			Func<Task> callLoop = async () =>
			{
				for (int i = 0; i < 4; i++)
					await api.Call("method.name", token: tokenSource.Token);
			};
			tokenSource.CancelAfter(TimeSpan.FromSeconds(2));
			var stopwatch = Stopwatch.StartNew();
			Assert.Throws<AggregateException>(() => Task.WaitAll(new[]
						 {
							 Task.Run(callLoop, tokenSource.Token),
							 Task.Run(callLoop, tokenSource.Token),
							 Task.Run(callLoop, tokenSource.Token)
						 }));
			stopwatch.Stop();

			Assert.That(Math.Abs(stopwatch.Elapsed.TotalSeconds - 2), Is.LessThan(0.3));
		}

		#endregion
	}
}