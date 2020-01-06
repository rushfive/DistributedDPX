using Common.Abstractions.TimeZones;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Abstractions.Dates
{
	//WARNING: Used as a Mongo Object
	public struct DsDateTime : IComparable, IComparable<DsDateTime>, IEquatable<DsDateTime>
	{
		public DsDate Date { get; }
		public DsTime Time { get; }

		public DsDateTime(DsDate date)
		{
			this.Date = date;
			this.Time = new DsTime();
		}

		public DsDateTime(DsDate date, DsTime time)
		{
			this.Date = date;
			this.Time = time;
		}

		public DsDateTime(int year, int month, int day)
			: this(year, month, day, 0, 0, 0, 0)
		{
		}

		public DsDateTime(int year, int month, int day, int hours)
			: this(year, month, day, hours, 0, 0, 0)
		{
		}

		public DsDateTime(int year, int month, int day, int hours, int minutes)
			: this(year, month, day, hours, minutes, 0, 0)
		{
		}

		public DsDateTime(int year, int month, int day, int hours, int minutes, int seconds)
			: this(year, month, day, hours, minutes, seconds, 0)
		{
		}

		public DsDateTime(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds)
		{
			this.Date = new DsDate(day, month, year);
			this.Time = new DsTime(hours, minutes, seconds, milliseconds);
		}

		public long Serialize()
		{
			long time = this.Time.Serialize();
			long date = this.Date.Serialize();
			//time will have 27 bits of values, the date will have 27 bits empty, then have y/m/d bits
			return date + time;
		}

		public static DsDateTime Deserialize(long serializedValue)
		{
			DsTime time = DsTime.Deserialize(serializedValue);
			DsDate date = DsDate.Deserialize(serializedValue);
			return new DsDateTime(date, time);
		}

		public static bool operator ==(DsDateTime dsTime1, DsDateTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) == 0;
		}

		public static bool operator !=(DsDateTime dsTime1, DsDateTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) != 0;
		}

		public static bool operator <(DsDateTime dsTime1, DsDateTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) == -1;
		}

		public static bool operator >(DsDateTime dsTime1, DsDateTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) == 1;
		}

		public static bool operator <=(DsDateTime dsTime1, DsDateTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) < 1;
		}

		public static bool operator >=(DsDateTime dsTime1, DsDateTime dsTime2)
		{
			return dsTime1.CompareTo(dsTime2) > -1;
		}

		public override int GetHashCode()
		{
			return this.Serialize().GetHashCode();
		}

		public int CompareTo(object obj)
		{
			if (obj is DsDateTime dt)
			{
				return this.CompareTo(dt);
			}
			return -1;
		}

		public int CompareTo(DsDateTime other)
		{
			int value = this.Date.CompareTo(other.Date);
			if (value != 0)
			{
				return value;
			}
			return this.Time.CompareTo(other.Time);
		}

		public bool Equals(DsDateTime other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			return this.CompareTo(obj) == 0;
		}

		public override string ToString()
		{
			return $"{this.Date}T{this.Time}";
		}

		public static DsDateTime Parse(string dateTimeString, bool canDeserializeNumber = false)
		{
			if (!DsDateTime.TryParse(dateTimeString, out DsDateTime dateTime, canDeserializeNumber))
			{
				throw new Exception($"'{dateTimeString}' is not a valid string value for a datetime.");
			}
			return dateTime;
		}

		public static bool TryParse(string dateTimeString, out DsDateTime dateTime, bool canDeserializeNumber = false)
		{
			if (canDeserializeNumber && long.TryParse(dateTimeString, out long serializedValue))
			{
				dateTime = DsDateTime.Deserialize(serializedValue);
				return true;
			}
			if (DateTimeOffset.TryParse(dateTimeString, null, DateTimeStyles.AssumeUniversal, out DateTimeOffset result))
			{
				var d = new DsDate(result.Day, result.Month, result.Year);
				var t = new DsTime(result.Hour, result.Minute, result.Second, result.Millisecond);
				dateTime = new DsDateTime(d, t);
				return true;
			}
			Match match = DatesRegexPatterns.DateTimeRegex.Match(dateTimeString);
			if (!match.Success)
			{
				dateTime = new DsDateTime();
				return false;
			}
			string dateString = match.Groups["date"].Value;
			string timeString = match.Groups["time"].Value;
			if (!DsDate.TryParse(dateString, out DsDate date) || !DsTime.TryParse(timeString, out DsTime time))
			{
				dateTime = new DsDateTime();
				return false;
			}
			dateTime = new DsDateTime(date, time);
			return true;
		}

		public DayOfWeek GetDayOfWeek()
		{
			return this.Date.GetDayOfWeek();
		}

		public DsDateTime RoundToDayOfWeek(DayOfWeek dayOfWeek, bool roundDown = false)
		{
			DsDate date = this.Date.RoundToDayOfWeek(dayOfWeek, roundDown);
			return new DsDateTime(date, this.Time);
		}

		public static DsDateTime FromDateTime(DateTimeOffset dateTimeOffset)
		{
			DsDate date = DsDate.FromDateTime(dateTimeOffset, nextFullDay: false);
			var time = new DsTime(dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond);
			return new DsDateTime(date, time);
		}

		public static DsDateTime FromDateTime(DateTime dateTime, PlatformTimeZone timeZone)
		{
			DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero)
				.ToOffset(timeZone.GetOffset(dateTime));
			return DsDateTime.FromDateTime(dateTimeOffset);
		}

		public DsDateTime Add(TimeSpan timeSpan)
		{
			(DsTime time, int days) = this.Time.AddWithOverflow(timeSpan);
			DsDate date = this.Date.AddDays(days);
			return new DsDateTime(date, time);
		}

		public DsDateTime AddSeconds(int seconds)
		{
			return this.Add(TimeSpan.FromSeconds(seconds));
		}

		public DsDateTime AddMinutes(int minutes)
		{
			return this.Add(TimeSpan.FromMinutes(minutes));
		}

		public DsDateTime AddHours(int hours)
		{
			return this.Add(TimeSpan.FromHours(hours));
		}

		public DsDateTime AddDays(int days)
		{
			DsDate date = this.Date.AddDays(days);
			return new DsDateTime(date, this.Time);
		}

		public DsDateTime AddWeeks(int weeks)
		{
			return this.AddDays(weeks * 7);
		}

		public DsDateTime AddMonths(int months)
		{
			DsDate date = this.Date.AddMonths(months);
			return new DsDateTime(date, this.Time);
		}

		public DsDateTime AddYears(int years)
		{
			DsDate date = this.Date.AddYears(years);
			return new DsDateTime(date, this.Time);
		}

		public DateTime ToUtcDateTime(PlatformTimeZone timeZone)
		{
			return this.ToDateTimeOffset(timeZone).UtcDateTime;
		}

		public DateTimeOffset ToDateTimeOffset()
		{
			return this.ToDateTimeOffset(GlobalizationUtil.GetUtcTimeZone());
		}

		public DateTimeOffset ToDateTimeOffset(PlatformTimeZone timeZone)
		{
			TimeSpan offset = timeZone.GetOffset(this);
			return new DateTimeOffset(this.Date.Year, this.Date.Month, this.Date.Day, this.Time.Hour, this.Time.Minutes, this.Time.Seconds, this.Time.Milliseconds, offset);
		}

		public static DsDateTime GetNow(PlatformTimeZone timeZone)
		{
			DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(timeZone.GetCurrentOffset());
			return DsDateTime.FromDateTime(now);
		}

		public bool IsInPast(PlatformTimeZone timeZone, bool includeNowAsPast = false)
		{
			DsDateTime now = DsDateTime.GetNow(timeZone);
			return includeNowAsPast ? this <= now : this < now;
		}

		public bool IsInFuture(PlatformTimeZone timeZone, bool includeNowAsFuture = false)
		{
			DsDateTime now = DsDateTime.GetNow(timeZone);
			return includeNowAsFuture ? this >= now : this > now;
		}

		public DsDateTime TruncateToMinutes()
		{
			DsTime time = this.Time.TruncateToMinutes();
			return new DsDateTime(this.Date, time);
		}

		public static explicit operator DsDate(DsDateTime dateTime)
		{
			return dateTime.Date;
		}

		public static implicit operator DsDateTime(DsDate date)
		{
			return new DsDateTime(date);
		}
	}
}
