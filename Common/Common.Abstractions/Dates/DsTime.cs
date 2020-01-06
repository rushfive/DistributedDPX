using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Abstractions.Dates
{
	//WARNING: Used as a Mongo Object
	public struct DsTime : IComparable, IComparable<DsTime>, IEquatable<DsTime>
	{
		public DsTime(int hours, int minutes)
			: this(hours, minutes, 0, 0)
		{
		}

		public DsTime(int hours, int minutes, int seconds)
			: this(hours, minutes, seconds, 0)
		{
		}

		public DsTime(int hours, int minutes, int seconds, int milliseconds)
		{
			if (hours >= 24 || hours < 0)
			{
				throw new ArgumentException("Hours must be 0->23.");
			}
			this.Hour = hours;
			if (minutes >= 60 || minutes < 0)
			{
				throw new ArgumentException("Minutes must be 0->59.");
			}
			this.Minutes = minutes;
			if (seconds >= 60 || seconds < 0)
			{
				throw new ArgumentException("Seconds must be 0->59.");
			}
			this.Seconds = seconds;
			if (milliseconds >= 1000 || milliseconds < 0)
			{
				throw new ArgumentException("Milliseconds must be 0->999.");
			}
			this.Milliseconds = milliseconds;
		}

		public int Hour { get; }

		public int Minutes { get; }

		public int Seconds { get; }

		public int Milliseconds { get; }

		public long Serialize()
		{
			//to millis
			TimeSpan ts = this.ToTimeSpan();
			//only can be 27 bits in size
			long maxMillis = (long)Math.Pow(2, 27);
			if (ts.TotalMilliseconds > maxMillis)
			{
				throw new Exception($"Time is too large to be serialized. Milliseconds - Value: {ts.TotalMilliseconds}, Max: {maxMillis}");
			}
			return (long)ts.TotalMilliseconds;
		}

		public static DsTime Deserialize(long serializedValue)
		{
			//27 bits are for the milliseconds of time, everything before is a date, so remove it
			long dateValue = serializedValue >> 27;
			serializedValue = serializedValue - (dateValue << 27);

			//from millis
			TimeSpan ts = TimeSpan.FromMilliseconds(serializedValue);

			return DsTime.FromTimespan(ts);
		}

		public static DsTime FromTimespan(TimeSpan timeSpan)
		{
			return new DsTime(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		}

		public static DsTime Parse(string timeString, bool canDeserializeNumber = false)
		{
			if (!DsTime.TryParse(timeString, out DsTime time, canDeserializeNumber))
			{
				throw new Exception($"'{timeString}' is not a valid string value for a time.");
			}
			return time;
		}

		public static bool TryParse(string timeString, out DsTime time, bool canDeserializeNumber = false)
		{
			if (canDeserializeNumber && long.TryParse(timeString, out long serializedValue))
			{
				time = DsTime.Deserialize(serializedValue);
				return true;
			}
			if (!DateTimeOffset.TryParse(timeString, null, DateTimeStyles.AssumeUniversal, out DateTimeOffset result))
			{
				Match match = DatesRegexPatterns.TimeRegex.Match(timeString);
				if (!match.Success)
				{
					time = new DsTime();
					return false;
				}
				int microseconds = 0;
				if (match.Groups["micro"].Success)
				{
					string microValue = match.Groups["micro"].Value;
					microValue = microValue
						//pad zeros to make .1 the same as .100000
						.PadRight(6, '0')
						//Remove leading zeros for parsing
						.TrimStart('0');
					microseconds = int.Parse(microValue);
				}

				//TODO accept micro vs millis?
				int milliseconds = (int)Math.Round(microseconds / 1000f);
				time = new DsTime(int.Parse(match.Groups["h"].Value), int.Parse(match.Groups["m"].Value), int.Parse(match.Groups["s"].Value), milliseconds);
				return true;
			}
			time = new DsTime(result.Hour, result.Minute, result.Second, result.Millisecond);
			return true;
		}

		public override string ToString()
		{
			return $"{this.Hour.ToString("D2")}:{this.Minutes.ToString("D2")}:{this.Seconds.ToString("D2")}.{this.Milliseconds.ToString("D3")}";
		}

		public int CompareTo(DsTime other)
		{
			long thisVal = this.Serialize();
			long otherVal = other.Serialize();

			if (thisVal < otherVal)
			{
				return -1;
			}
			if (thisVal > otherVal)
			{
				return 1;
			}
			return 0;
		}

		public int CompareTo(object other)
		{
			if (!(other is DsTime))
			{
				return -1;
			}
			return this.CompareTo((DsTime)other);
		}

		public bool Equals(DsTime other)
		{
			return this.Serialize() == other.Serialize();
		}

		public override bool Equals(object other)
		{
			if (!(other is DsTime))
			{
				return false;
			}

			return this.Equals((DsTime)other);
		}

		public static bool operator ==(DsTime dsTime1, DsTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) == 0;
		}

		public static bool operator !=(DsTime dsTime1, DsTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) != 0;
		}

		public static bool operator <(DsTime dsTime1, DsTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) == -1;
		}

		public static bool operator >(DsTime dsTime1, DsTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) == 1;
		}

		public static bool operator <=(DsTime dsTime1, DsTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) < 1;
		}

		public static bool operator >=(DsTime dsTime1, DsTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) > -1;
		}

		public override int GetHashCode()
		{
			return this.Serialize().GetHashCode();
		}

		public DsTime Add(DsTime other, DsTimeAddType addType = DsTimeAddType.ThrowOnOverflow)
		{
			TimeSpan ts = other.ToTimeSpan();
			return this.Add(ts, addType);
		}

		public DsTime Add(TimeSpan span, DsTimeAddType addType = DsTimeAddType.ThrowOnOverflow)
		{
			(DsTime time, int days) = this.AddWithOverflow(span);

			if (days > 0)
			{
				switch (addType)
				{
					case DsTimeAddType.ThrowOnOverflow:
						throw new Exception("Time overflowed past 24 hours.");
					case DsTimeAddType.MaxOutInsteadOfOverflow:
						return new DsTime(23, 59, 59, 999);
					default:
						throw new Exception();
				}
			}
			return time;
		}

		public (DsTime Time, int Days) AddWithOverflow(TimeSpan timeSpan)
		{
			TimeSpan ts = this.ToTimeSpan();
			ts = ts.Add(timeSpan);

			int days = 0;
			while (ts.TotalDays > 0)
			{
				ts = ts.Add(TimeSpan.FromDays(-1));
				days++;
			}
			while (ts.TotalDays < 0)
			{
				ts = ts.Add(TimeSpan.FromDays(1));
				days--;
			}

			return (DsTime.FromTimespan(ts), days);
		}

		public DsTime AddHours(int hours, DsTimeAddType addType = DsTimeAddType.ThrowOnOverflow)
		{
			return this.Add(new TimeSpan(hours, 0, 0), addType);
		}

		public TimeSpan ToTimeSpan()
		{
			return new TimeSpan(0, this.Hour, this.Minutes, this.Seconds, this.Milliseconds);
		}

		public DsTime TruncateToMinutes()
		{
			return new DsTime(this.Hour, this.Minutes);
		}
	}

	public enum DsTimeAddType
	{
		ThrowOnOverflow,
		MaxOutInsteadOfOverflow,
		OverflowButRemoveDays
	}
}
