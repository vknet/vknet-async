using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VkNetAsync.Annotations;
using VkNetAsync.Service.Captcha;
using VkNetAsync.Service.Network;

namespace VkNetAsync.API.Authorisation
{
	public abstract class Authoriser : INotifyPropertyChanged
	{
		public static VkApi FromToken(long userId, [NotNull] string accessToken, [NotNull] INetworkTransport transport, [CanBeNull] ICaptchaResolver captchaResolver = null)
		{
			Contract.Requires<ArgumentNullException>(transport != null);
			Contract.Requires<ArgumentOutOfRangeException>(userId > 0);
			Contract.Requires<ArgumentNullException>(accessToken != null);
			Contract.Requires<FormatException>(Regex.IsMatch(accessToken, @"^[0-9a-f]+$"));

			return new VkApi(userId, accessToken, transport, captchaResolver);
		}

		private readonly INetworkTransport _transport;
		public INetworkTransport Transport
		{
			get { return _transport; }
		}

		private readonly ICaptchaResolver _captchaResolver;
		public ICaptchaResolver CaptchaResolver
		{
			get { return _captchaResolver; }
		}

		private VkApi _api;
		public VkApi API
		{
			get { return _api; }
			protected set
			{
				if (Equals(value, _api)) return;
				if ((_api = value) != null)
					OnAuthorised();
				OnPropertyChanged();
			}
		}

		protected Authoriser([NotNull] INetworkTransport transport, [CanBeNull] ICaptchaResolver captchaResolver = null)
		{
			Contract.Requires<ArgumentNullException>(transport != null);

			_transport = transport;
			_captchaResolver = captchaResolver;
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