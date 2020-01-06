using Common.Abstractions.TimeZones;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Abstractions.Dates
{
	//WARNING: Used as a Mongo Object
	public struct DsDate : IComparable, IComparable<DsDate>, IEquatable<DsDate>
	{
		public DsDate(int day, int month, int year)
		{
			//todo: max values
			this.Day = Fix(day, 31);
			this.Month = Fix(month, 12);
			this.Year = Fix(year, null);

			int Fix(int value, int? max)
			{
				if (value < 1)
				{
					value = 1;
				}

				if (max.HasValue && value > max)
				{
					value = max.Value;
				}

				return value;
			}
		}

		public int Day { get; }

		public int Month { get; }

		public int Year { get; }

		public long Serialize()
		{
			long yearShift = (long)this.Year << 36;
			long monthShift = (long)this.Month << 32;
			long dayShift = (long)this.Day << 27;

			return yearShift + monthShift + dayShift;
		}

		public static DsDate Deserialize(long serializedValue)
		{
			//first x bits are year number
			//second 4 bits are the the month
			//third 5 bits are the day
			//fourth 27 bits are for the milliseconds of time (blank for date)
			long newYear = serializedValue >> 36;

			long sv2 = serializedValue - (newYear << 36);
			long newMonth = sv2 >> 32;

			long sv3 = sv2 - (newMonth << 32);
			long newDay = sv3 >> 27;

			return new DsDate((int)newDay, (int)newMonth, (int)newYear);
		}

		public static DsDate Parse(string dateString, bool canDeserializeNumber = false)
		{
			if (!DsDate.TryParse(dateString, out DsDate dsDate, canDeserializeNumber))
			{
				throw new Exception($"'{dateString}' is not a valid string value for a date.");
			}
			return dsDate;
		}

		public static bool TryParse(string dateString, out DsDate date, bool canDeserializeNumber = false)
		{
			if (canDeserializeNumber && long.TryParse(dateString, out long serializedValue))
			{
				date = DsDate.Deserialize(serializedValue);
				return true;
			}
			if (!DateTimeOffset.TryParse(dateString, null, DateTimeStyles.AssumeUniversal, out DateTimeOffset result))
			{
				Match match = DatesRegexPatterns.DateRegex.Match(dateString);
				if (!match.Success)
				{
					date = new DsDate();
					return false;
				}
				date = new DsDate(int.Parse(match.Groups["y"].Value), int.Parse(match.Groups["m"].Value), int.Parse(match.Groups["d"].Value));
				return true;
			}
			date = new DsDate(result.Day, result.Month, result.Year);
			return true;
		}

		public static DsDate GetToday(PlatformTimeZone timeZone)
		{
			DateTimeOffset today = DateTimeOffset.UtcNow.ToOffset(timeZone.GetCurrentOffset());
			return new DsDate(today.Day, today.Month, today.Year);
		}

		public static DsDate GetBeginningOfWeek(PlatformTimeZone timeZone, DayOfWeek firstDayOfWeek = DayOfWeek.Monday, DateTimeOffset? now = null, bool nextFullWeek = false)
		{
			now = now ?? DateTimeOffset.UtcNow;
			DateTimeOffset today = now.Value.ToOffset(timeZone.GetOffset(now.Value));
			today = today.RoundToDayOfWeek(firstDayOfWeek, onlyDate: true, roundDown: !nextFullWeek);
			return new DsDate(today.Day, today.Month, today.Year);
		}

		public static DsDate FromDateTime(DateTime dateTime, PlatformTimeZone timeZone, bool nextFullDay = false)
		{
			var today = new DateTimeOffset(dateTime, TimeSpan.Zero);
			if (timeZone != null)
			{
				today = today.ToOffset(timeZone.GetOffset(today));
			}
			return DsDate.FromDateTime(today, nextFullDay);
		}

		public static DsDate FromDateTime(DateTimeOffset dateTimeOffset, bool nextFullDay = false)
		{
			if (nextFullDay)
			{
				dateTimeOffset = dateTimeOffset.AddDays(1);
			}
			return new DsDate(dateTimeOffset.Day, dateTimeOffset.Month, dateTimeOffset.Year);
		}

		public DsDate AddDays(int days)
		{
			DateTimeOffset dateTime = this.ToDateTime().AddDays(days);
			return new DsDate(dateTime.Day, dateTime.Month, dateTime.Year);
		}

		public DsDate AddMonths(int months)
		{
			DateTimeOffset dateTime = this.ToDateTime().AddMonths(months);
			return new DsDate(dateTime.Day, dateTime.Month, dateTime.Year);
		}

		public DsDate AddYears(int years)
		{
			DateTimeOffset dateTime = this.ToDateTime().AddYears(years);
			return new DsDate(dateTime.Day, dateTime.Month, dateTime.Year);
		}

		public DayOfWeek GetDayOfWeek()
		{
			return this.ToDateTime(GlobalizationUtil.GetUtcTimeZone()).DayOfWeek;
		}

		public DateTimeOffset ToDateTime()
		{
			return this.ToDateTime(GlobalizationUtil.GetUtcTimeZone());
		}

		public DateTime ToUtcDateTime()
		{
			return this.ToDateTime(GlobalizationUtil.GetUtcTimeZone()).UtcDateTime;
		}

		public DateTimeOffset ToDateTime(PlatformTimeZone timeZone)
		{
			TimeSpan offset = timeZone.GetOffset(new DsDateTime(this, new DsTime()));
			return new DateTimeOffset(this.Year, this.Month, this.Day, 0, 0, 0, 0, offset);
		}

		public override string ToString()
		{
			return $"{this.Year:D4}-{this.Month:D2}-{this.Day:D2}";
		}

		public int CompareTo(DsDate other)
		{
			long thisVal = this.Serialize();
			long otherVal = other.Serialize();

			return thisVal.CompareTo(otherVal);
		}

		public int CompareTo(object other)
		{
			if (!(other is DsDate))
			{
				return -1;
			}
			return this.CompareTo((DsDate)other);
		}

		public bool Equals(DsDate other)
		{
			return this.Serialize() == other.Serialize();
		}

		public override bool Equals(object other)
		{
			if (!(other is DsDate))
			{
				return false;
			}

			return this.Equals((DsDate)other);
		}

		public static bool operator ==(DsDate dsDate1, DsDate dsDate2)
		{
			return dsDate1.CompareTo(dsDate2) == 0;
		}

		public static bool operator !=(DsDate dsDate1, DsDate dsDate2)
		{
			return dsDate1.CompareTo(dsDate2) != 0;
		}

		public static bool operator <(DsDate dsDate1, DsDate dsDate2)
		{
			return dsDate1.CompareTo(dsDate2) == -1;
		}

		public static bool operator >(DsDate dsDate1, DsDate dsDate2)
		{
			return dsDate1.CompareTo(dsDate2) == 1;
		}

		public static bool operator <=(DsDate dsDate1, DsDate dsDate2)
		{
			return dsDate1.CompareTo(dsDate2) < 1;
		}

		public static bool operator >=(DsDate dsDate1, DsDate dsDate2)
		{
			return dsDate1.CompareTo(dsDate2) > -1;
		}

		public static TimeSpan operator -(DsDate dsDate1, DsDate dsDate2)
		{
			return dsDate1.ToUtcDateTime() - dsDate2.ToUtcDateTime();
		}

		public override int GetHashCode()
		{
			return this.Serialize().GetHashCode();
		}

		public int GetDayDifference(DsDate dsDate, bool absoluteValue = true)
		{
			TimeSpan diff = this.ToDateTime() - dsDate.ToDateTime();
			int days = (int)Math.Ceiling(diff.TotalDays);
			if (!absoluteValue)
			{
				return days;
			}
			return Math.Abs(days);
		}

		public DsDate RoundToDayOfWeek(DayOfWeek dayOfWeek, bool roundDown = false)
		{
			DateTimeOffset dateTime = this.ToDateTime().RoundToDayOfWeek(dayOfWeek, onlyDate: true, roundDown: roundDown);
			return DsDate.FromDateTime(dateTime, nextFullDay: false);
		}
	}
}
