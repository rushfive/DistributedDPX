using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Abstractions
{
	public struct OptionalValue<T>
	{
		private readonly T value;

		public bool IsSpecified { get; }

		public OptionalValue(T value) : this(value, true)
		{
		}

		private OptionalValue(T value, bool isSpecified)
		{
			this.value = value;
			this.IsSpecified = isSpecified;
		}

		public T Value
		{
			get
			{
				if (!this.IsSpecified)
				{
					throw new InvalidOperationException("Value does not exist because optional value is not specified.");
				}
				return this.value;
			}
		}

		public T GetValueOrDefault(T defaultValue = default)
		{
			return this.IsSpecified ? this.Value : defaultValue;
		}

		public static OptionalValue<T> WithValue(T value)
		{
			return new OptionalValue<T>(value);
		}

		public static implicit operator OptionalValue<T>(T value)
		{
			return new OptionalValue<T>(value);
		}

		public static explicit operator T(OptionalValue<T> value)
		{
			return value.Value;
		}

		public override bool Equals(object other)
		{
			if (!this.IsSpecified)
			{
				//Shouldn't equal something else even if other is null
				return false;
			}
			if (this.Value == null && other == null)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			return other.Equals(this.Value);
		}

		public override int GetHashCode()
		{
			return this.IsSpecified && this.value != null ? this.value.GetHashCode() : 0;
		}

		public override string ToString()
		{
			return this.IsSpecified ? this.value.ToString() : "";
		}

		public static OptionalValue<T> NotSpecified { get; } = new OptionalValue<T>(default, false);
	}
}
