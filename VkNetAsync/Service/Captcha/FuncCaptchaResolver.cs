using System;
using System.Diagnostics.Contracts;
using VkNetAsync.Annotations;

namespace VkNetAsync.Service.Captcha
{
	class FuncCaptchaResolver : ICaptchaResolver
	{
		private readonly Func<Captcha, string> _resolver;

		public FuncCaptchaResolver([NotNull] Func<Captcha, string> resolver)
		{
			Contract.Requires<ArgumentNullException>(resolver != null);

			_resolver = resolver;
		}

		public string Resolve(Captcha captcha)
		{
			return _resolver(captcha);
		}
	}
}