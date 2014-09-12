using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VkNetAsync.Annotations;
using VkNetAsync.Service.Network;

namespace VkNetAsync.API.Authorisation
{
	public abstract class Authoriser : INotifyPropertyChanged
	{
		public static VkApi FromToken(long userId, [NotNull] string accessToken, [NotNull] INetworkTransport transport)
		{
			Contract.Requires<ArgumentNullException>(transport != null);
			Contract.Requires<ArgumentOutOfRangeException>(userId > 0);
			Contract.Requires<ArgumentNullException>(accessToken != null);
			Contract.Requires<FormatException>(Regex.IsMatch(accessToken, @"^[0-9a-f]+$"));

			return new VkApi(userId, accessToken, transport);
		}

		private readonly INetworkTransport _transport;
		public INetworkTransport Transport
		{
			get { return _transport; }
		}

		private VkApi _api;
		public VkApi API
		{
			get { return _api; }
			private set
			{
				if (Equals(value, _api)) return;
				if ((_api = value) != null)
					OnAuthorised();
				OnPropertyChanged();
			}
		}

		protected Authoriser([NotNull] INetworkTransport transport)
		{
			Contract.Requires<ArgumentNullException>(transport != null);
			
			_transport = transport;
		}

		public event EventHandler Authorised;
		
		protected virtual void OnAuthorised()
		{
			var handler = Authorised;
			if (handler != null) handler(this, EventArgs.Empty);
		}


		#region INPC

		public event PropertyChangedEventHandler PropertyChanged;
		
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}