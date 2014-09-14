using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using VkNetAsync.Annotations;

namespace VkNetAsync.API.VkTypes.Enums
{
	public class FilteredEnum<TFilter> where TFilter : FilteredEnum<TFilter>, new()
	{
		private sealed class MembersContainer<TContainer> : ExclusiveEnum<MembersContainer<TFilter>> where TContainer : FilteredEnum<TFilter>
		{
			public new IList<MembersContainer<TFilter>> PossibleValues
			{
				get { return ExclusiveEnum<MembersContainer<TFilter>>.PossibleValues; }
			}

			public MembersContainer<TFilter> MemberWithValueOrDefault(long value)
			{
				return PossibleValues.SingleOrDefault(member => member.Value == value);
			}

			public MembersContainer<TFilter> MemberWithNameOrDefault(string name)
			{
				return PossibleValues.SingleOrDefault(member => member.Name == name);
			}

			public TFilter Register(long value, string name)
			{
				return new TFilter() { _value = RegisterPossibleValue(value, name).Value };
			}
		}

		#region Static members

		// ReSharper disable once StaticFieldInGenericType
		private static readonly MembersContainer<FilteredEnum<TFilter>> ExclusiveMembersContainer = new MembersContainer<FilteredEnum<TFilter>>();

		protected static TFilter RegisterPossibleValue(long value, [NotNull] string name)
		{
			Contract.Requires<ArgumentNullException>(name != null);
			Contract.Requires<ArgumentException>(name.Length > 0);

			if (value == 0 || (value & (value - 1)) != 0)
				throw new ArgumentException(string.Format("Value {0} must be a power of 2 (i.e. only one bit must be equal to 1)", Convert.ToString(value, 2)), "value");

			var member = ExclusiveMembersContainer.MemberWithValueOrDefault(value);
			if (member != null)
				throw new ArgumentException(string.Format("Element of type {0} with value {1} is already registered ({2}).",
														  typeof(TFilter).FullName, value, member),
											"value");

			member = ExclusiveMembersContainer.MemberWithNameOrDefault(name);
			if (member != null)
				throw new ArgumentException(string.Format("Element of type {0} with name {1} is not registered ({2}).",
														  typeof(TFilter).FullName, name, member),
											"name");

			return ExclusiveMembersContainer.Register(value, name);
		}

		protected static TFilter FromValue(long value)
		{
			var registeredMask = ExclusiveMembersContainer.PossibleValues.Select(container => container.Value).Sum();
			for (long mask = 1; mask != 0; mask = mask << 1)
			{
				if((value & mask) == 0)
					continue;
				if((registeredMask & mask) == 0)
				  throw new ArgumentException(
						string.Format("Value {0} has bits that not defined for type {1} (defined bits: {2})",
						Convert.ToString(value, 2), typeof(TFilter).FullName, Convert.ToString(registeredMask, 2)), "value");
			}

			return new TFilter() { _value = value };
		}

		#endregion

		private long _value;
		public long Value
		{
			get { return _value; }
		}

		protected FilteredEnum()
		{
			_value = 0;
		}

		public override string ToString()
		{
			return string.Join(",", ExclusiveMembersContainer.PossibleValues.Where(member => (member.Value & Value) != 0).Select(member => member.Name));
		}

		public static TFilter operator |(FilteredEnum<TFilter> left, FilteredEnum<TFilter> right)
		{
			return new TFilter() { _value = (left.Value | right.Value) };
		}

		public static bool operator ==(FilteredEnum<TFilter> left, FilteredEnum<TFilter> right)
		{
			if (ReferenceEquals(right, left)) return true;
			if (ReferenceEquals(null, left)) return false;
			if (ReferenceEquals(null, right)) return false;

			return left.Value == right.Value;
		}

		public static bool operator !=(FilteredEnum<TFilter> left, FilteredEnum<TFilter> right)
		{
			return !(left == right);
		}

		protected bool Equals(FilteredEnum<TFilter> other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((FilteredEnum<TFilter>)obj);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}


	}
}