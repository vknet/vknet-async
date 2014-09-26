using System;
using System.Diagnostics.Contracts;
using VkNetAsync.Annotations;

namespace VkNetAsync.Service.Captcha
{
	public sealed class Captcha
	{
		public long Sid { get; private set; }
		
		[NotNull] 
		public Uri Image { get; private set; }

		public Captcha(long sid, [NotNull] Uri image)
		{
			Contract.Requires<ArgumentOutOfRangeException>(sid > 0);
			Contract.Requires<ArgumentNullException>(image != null);
			
			Sid = sid;
			Image = image;
		}

		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(Image != null);
			Contract.Invariant(Sid > 0);
		}
	}
}