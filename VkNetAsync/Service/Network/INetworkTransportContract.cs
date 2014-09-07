using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace VkNetAsync.Service.Network
{
	[ContractClassFor(typeof (INetworkTransport))]
	abstract class INetworkTransportContract : INetworkTransport
	{
		Task<string> INetworkTransport.Post(Uri uri, CancellationToken token)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			
			throw new System.NotImplementedException();
		}
	}
}