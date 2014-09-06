using System;
using NUnit.Framework;
using VkNetAsync.Service;

namespace VkNetAsync.Tests.Service
{
	[TestFixture]
	public class DateTimeExTests
	{
		[Test]
		public void ToUnixTime_CorrectDateTime()
		{
			Assert.That(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTime(), Is.EqualTo(0));
			Assert.That(new DateTime(2014, 9, 6, 22, 15, 10, DateTimeKind.Utc).ToUnixTime(), Is.EqualTo(1410041710));
		}

		[Test]
		public void ToUnixTime_IncorrectDateTime()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1969, 12, 31, 23, 59, 59, DateTimeKind.Utc).ToUnixTime());
		}


		[Test]
		public void FromUnixTime_CorrectTimestamp()
		{
			Assert.That(DateTimeEx.UnixTimeToDateTime(0), Is.EqualTo(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
			Assert.That(DateTimeEx.UnixTimeToDateTime(1410041710), Is.EqualTo(new DateTime(2014, 9, 6, 22, 15, 10, DateTimeKind.Utc)));
		}

		[Test]
		public void FromUnixTime_IncorrectDateTime()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeEx.UnixTimeToDateTime(-1));
			Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeEx.UnixTimeToDateTime(long.MinValue));
		}


	}
}