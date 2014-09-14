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
		Task<Response> Post([NotNull]Uri uri, [CanBeNull] byte[] data, CancellationToken token = default(CancellationToken));
		
		Task<Response> Get([NotNull]Uri uri, CancellationToken token = default(CancellationToken));
	}
}