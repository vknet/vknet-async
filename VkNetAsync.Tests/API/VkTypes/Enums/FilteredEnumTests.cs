using System;
using NUnit.Framework;
using VkNetAsync.API.VkTypes.Enums;

namespace VkNetAsync.Tests.API.VkTypes.Enums
{
	[TestFixture]
	public class FilteredEnumTests : FilteredEnum<FilteredEnumTests>
	{
		#region RegistrationTests

		[TestFixture]
		public class FilteredEnumRegistrationTests : FilteredEnum<FilteredEnumRegistrationTests>
		{
			[Test]
			public void Register_NotRegisteredCorrectMember_DoesNotThrow()
			{
				Assert.DoesNotThrow(() => RegisterPossibleValue(1, "member1"));
			}

			[Test]
			public void Register_NotRegisteredCorrectMember_ReturnsRegisteredMember()
			{
				Assert.That(RegisterPossibleValue(2, "member2"), Is.EqualTo(FromValue(2)));
			}

			[Test]
			public void Register_NotRegisteredMemberWithIncorrectValue_ReturnsRegisteredMember()
			{
				Assert.Throws<ArgumentException>(() => RegisterPossibleValue(3, "member3"));
				Assert.Throws<ArgumentException>(() => RegisterPossibleValue(0, "member0"));
			}

			[Test]
			public void Register_MemberWithTheSameValueExists_ThrowsArgumentException()
			{
				RegisterPossibleValue(4, "member4");
				Assert.Throws<ArgumentException>(() => RegisterPossibleValue(4, "memberName"));
			}

			[Test]
			public void Register_MemberWithTheSameNameExists_ThrowsArgumentException()
			{
				RegisterPossibleValue(8, "member8");
				Assert.Throws<ArgumentException>(() => RegisterPossibleValue(16, "member8"));
			}

		}

		#endregion

		private static readonly FilteredEnumTests MemberA = RegisterPossibleValue(1, "member(0*)1_name");
		private static readonly FilteredEnumTests MemberB = RegisterPossibleValue(2, "member(0*)10_name");
		private static readonly FilteredEnumTests MemberC = RegisterPossibleValue(16, "member(0*)10000_name");
		private static readonly FilteredEnumTests MemberD = RegisterPossibleValue(1L << 62, "member01(0*)_name");
		private static readonly FilteredEnumTests MemberE = RegisterPossibleValue(1L << 63, "member1(0*)_name");

		#region FromValue

		[Test]
		public void FromValue_RegisteredValue_ReturnsRegisteredMember()
		{
			Assert.That(FromValue(1), Is.EqualTo(MemberA));
			Assert.That(FromValue(2), Is.EqualTo(MemberB));
			Assert.That(FromValue(16), Is.EqualTo(MemberC));
			Assert.That(FromValue(1L << 62), Is.EqualTo(MemberD));
			Assert.That(FromValue(1L << 63), Is.EqualTo(MemberE));
		}

		[Test]
		public void FromValue_NotRegisteredValue_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => FromValue(4));
			Assert.Throws<ArgumentException>(() => FromValue(8));
		}

		[Test]
		public void FromValue_CombinedValue_ReturnsCorrectValue()
		{
			Assert.That(FromValue(3).Value, Is.EqualTo(3));
			Assert.That(FromValue(19).Value, Is.EqualTo(19));
		}

		[Test]
		public void FromValue_CombinedValueWithNotRegisteredBits_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => FromValue(10));
			Assert.Throws<ArgumentException>(() => FromValue(3L << 61));
		}



		#endregion

		[Test]
		public void ValueProperty_CombinedFilter_ReturnsCorrectValue()
		{
			Assert.That((MemberA | MemberB).Value, Is.EqualTo(3));
			Assert.That((MemberA | MemberB | MemberC).Value, Is.EqualTo(19));
			Assert.That((MemberD | MemberE).Value, Is.EqualTo(3L << 62));
		}

		[Test]
		public void ToString_FilterWithSingleValue_ReturnsStringRepresentationOfFilter()
		{
			Assert.That(MemberA.ToString(), Is.EqualTo("member(0*)1_name"));
			Assert.That(MemberD.ToString(), Is.EqualTo("member01(0*)_name"));
		}

		[Test]
		public void ToString_CombinedFilter_ReturnsStringRepresentationOfFilter()
		{
			Assert.That((MemberB | MemberC).ToString(), Is.EqualTo("member(0*)10_name,member(0*)10000_name"));
			Assert.That((MemberA | MemberE | MemberD).ToString(), Is.EqualTo("member(0*)1_name,member01(0*)_name,member1(0*)_name"));
		}

		[Test]
		public void OrBitOperator_FiltersWithSomeSameMembers_CombinesCorrectly()
		{
			Assert.True((MemberA | MemberB | MemberA) == (MemberA | MemberB));
			Assert.True((MemberA | MemberA) == (MemberA | MemberA | MemberA));
		}

		#region Equality

		[Test]
		public void Equality_EqualMembers_ReturnsTrue()
		{
			// ReSharper disable once EqualExpressionComparison
			Assert.True(MemberA == MemberA);
			Assert.True(MemberD.Equals(MemberD));
		}

		[Test]
		public void Equality_NotEqualMembers_ReturnsFalse()
		{
			// ReSharper disable once EqualExpressionComparison
			Assert.False(MemberB == MemberC);
			Assert.False(MemberE.Equals(MemberA));
			Assert.True(MemberC != MemberA);
		}

		#endregion
	}




}