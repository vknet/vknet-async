using System;

namespace VkNetAsync.Service.Captcha
{
	public abstract class CaptchaResolvingCancelledException : System.Exception
	{
        public Captcha Captcha { get; private set; }

		public CaptchaResolvingCancelledException(string message, Captcha captcha) : base(message)
		{
			Captcha = captcha;
		}
	}

}