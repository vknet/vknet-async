using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace VkNetAsync.Service.Network
{
	[ContractClassFor(typeof (INetworkTransport))]
	abstract class INetworkTransportContract : INetworkTransport
	{
		public Task<Response> Post(Uri uri, byte[] data, CancellationToken token = new CancellationToken())
		{
			Contract.Requires<ArgumentNullException>(uri != null);

			throw new System.NotImplementedException();
		}

		public Task<Response> Get(Uri uri, CancellationToken token)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			
			throw new System.NotImplementedException();
		}
	}
}