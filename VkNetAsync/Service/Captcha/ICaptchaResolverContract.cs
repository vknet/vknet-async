using System;
using System.Diagnostics.Contracts;

namespace VkNetAsync.Service.Captcha
{
	[ContractClassFor(typeof (ICaptchaResolver))]
	abstract class ICaptchaResolverContract : ICaptchaResolver
	{
		public string Resolve(Captcha captcha)
		{
			Contract.Requires<ArgumentNullException>(captcha != null);
			
			throw new System.NotImplementedException();
		}
	}
}