using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using VkNetAsync.Annotations;

namespace VkNetAsync.Service.Network
{
	[ContractClass(typeof (INetworkTransportContract))]
	public interface INetworkTransport
	{
		Task<string> Post([NotNull]Uri url, CancellationToken token = default(CancellationToken));
	}
}