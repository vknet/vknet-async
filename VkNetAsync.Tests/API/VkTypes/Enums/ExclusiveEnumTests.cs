using System;
using NUnit.Framework;
using VkNetAsync.API.VkTypes.Enums;

namespace VkNetAsync.Tests.API.VkTypes.Enums
{
	[TestFixture]
	public class ExclusiveEnumTests : ExclusiveEnum<ExclusiveEnumTests>
	{
		#region RegistrationTests

		[TestFixture]
		public class ExclusiveEnumRegistrationTests : ExclusiveEnum<ExclusiveEnumRegistrationTests>
		{
			[Test]
			public void Register_NotRegisteredMember_DoesNotThrow()
			{
				Assert.DoesNotThrow(() => RegisterPossibleValue(1, "member1"));
			}

			[Test]
			public void Register_NotRegisteredMember_ReturnsRegisteredMember()
			{
				Assert.That(RegisterPossibleValue(2, "member2"), Is.SameAs(FromValue(2)));
			}

			[Test]
			public void Register_MemberWithTheSameValueExists_ThrowsArgumentException()
			{
				RegisterPossibleValue(3, "member3");
				Assert.Throws<ArgumentException>(() => RegisterPossibleValue(3, "memberName"));
			}

			[Test]
			public void Register_MemberWithTheSameNameExists_ThrowsArgumentException()
			{
				RegisterPossibleValue(4, "member4");
				Assert.Throws<ArgumentException>(() => RegisterPossibleValue(5, "member4"));
			}

		}

		#endregion


		private static readonly ExclusiveEnumTests MemberA =   RegisterPossibleValue(0, "memberA_name");
		private static readonly ExclusiveEnumTests MemberB =   RegisterPossibleValue(10, "memberB_name");
		private static readonly ExclusiveEnumTests MemberC =   RegisterPossibleValue(-5, "memberC_name");
		private static readonly ExclusiveEnumTests MemberMax = RegisterPossibleValue(long.MaxValue, "memberMax_name");
		private static readonly ExclusiveEnumTests MemberMin = RegisterPossibleValue(long.MinValue, "memberMin_name");

		#region FromValue

		[Test]
		public void FromValue_RegisteredValue_ReturnsRegisteredMember()
		{
			Assert.That(FromValue(0), Is.SameAs(MemberA));
			Assert.That(FromValue(10), Is.SameAs(MemberB));
			Assert.That(FromValue(-5), Is.SameAs(MemberC));
			Assert.That(FromValue(long.MaxValue), Is.SameAs(MemberMax));
			Assert.That(FromValue(long.MinValue), Is.SameAs(MemberMin));
		}

		[Test]
		public void FromValue_NotRegisteredValue_ThrowsArgumentOutOfRangeException()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => FromValue(-1));
			Assert.Throws<ArgumentOutOfRangeException>(() => FromValue(-10));
		}

		#endregion


		#region FromName

		[Test]
		public void FromName_RegisteredName_ReturnsRegisteredMember()
		{
			Assert.That(FromName("memberA_name"), Is.SameAs(MemberA));
			Assert.That(FromName("memberB_name"), Is.SameAs(MemberB));
			Assert.That(FromName("memberC_name"), Is.SameAs(MemberC));
			Assert.That(FromName("memberMax_name"), Is.SameAs(MemberMax));
			Assert.That(FromName("memberMin_name"), Is.SameAs(MemberMin));
		}

		[Test]
		public void FromName_NotRegisteredName_ThrowsArgumentOutOfRangeException()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => FromName("notExisted"));
			Assert.Throws<ArgumentOutOfRangeException>(() => FromName("notExistedToo"));
		}

		[Test]
		public void FromName_NullName_ThrowsArgumentNullException()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Assert.Throws<ArgumentNullException>(() => FromName(null));
		}

		[Test]
		public void FromName_EmptyName_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => FromName(string.Empty));
		}

		#endregion


		#region Properties

		[Test]
		public void ValueProperty_ReturnsCorrectValue()
		{
			Assert.That(MemberA.Value, Is.EqualTo(0));
			Assert.That(MemberB.Value, Is.EqualTo(10));
			Assert.That(MemberC.Value, Is.EqualTo(-5));
			Assert.That(MemberMax.Value, Is.EqualTo(long.MaxValue));
			Assert.That(MemberMin.Value, Is.EqualTo(long.MinValue));
		}

		[Test]
		public void NameProperty_ReturnsCorrectName()
		{
			Assert.That(MemberA.Name, Is.EqualTo("memberA_name"));
			Assert.That(MemberB.Name, Is.EqualTo("memberB_name"));
			Assert.That(MemberC.Name, Is.EqualTo("memberC_name"));
			Assert.That(MemberMax.Name, Is.EqualTo("memberMax_name"));
			Assert.That(MemberMin.Name, Is.EqualTo("memberMin_name"));
		}

		#endregion


		#region Equality

		[Test]
		public void Equality_EqualMembers_ReturnsTrue()
		{
			// ReSharper disable once EqualExpressionComparison
			Assert.True(MemberA == MemberA);
			Assert.True(MemberMax.Equals(MemberMax));
		}

		[Test]
		public void Equality_NotEqualMembers_ReturnsFalse()
		{
			// ReSharper disable once EqualExpressionComparison
			Assert.False(MemberB == MemberC);
			Assert.False(MemberMax.Equals(MemberMin));
			Assert.True(MemberC != MemberA);
		}

		#endregion
	}




}