using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace sh04Zahorulko
{
    public enum ChineseZodiac
    {
        Rat, Ox, Tiger, Rabbit, Dragon, Snake, Horse, Goat, Monkey, Rooster, Dog, Pig
    }
    public enum WesternZodiac
    {
        Water, Fish, Ram, Bull, Twins, Crab, Lion, Maiden, Scales, Scorpion, Archer, Goat
    }

    [Serializable]
    public class BirthdayDate : IComparable
    {
        public DateTime Date { get; private set; }
        public WesternZodiac WesternZodiac { get; }
        public ChineseZodiac ChineseZodiac { get; }
        public int Years { get; }

        private BirthdayDate(DateTime date)
        {
            Date = date;
            WesternZodiac = WesternFromDateTime(date);
            ChineseZodiac = ChineseFromDateTime(date);
            Years = YearDifference(DateTime.Today, date);
        }

        public bool IsBirthDay(DateTime at)
        {
            return Date.Month == at.Month && Date.Day == at.Day;
        }

        public static WesternZodiac WesternFromDateTime(in DateTime date)
        {
            var normalizedDate = new DateTime(2000, date.Month, date.Day);
            ReadOnlySpan<int> increments = stackalloc[] { 29, 30, 29, 30, 31, 30, 30, 30, 29, 30, 28 };
            var previous = new DateTime(2000, 1, 20);
            int? selected = null;
            for (int i = 0; i < increments.Length; i++)
            {
                var next = previous.AddDays(increments[i]).AddDays(1);
                if (normalizedDate < next && normalizedDate >= previous)
                    selected = i;
                previous = next;

            }
            selected ??= increments.Length;

            return (WesternZodiac)selected;
        }

        public static ChineseZodiac ChineseFromDateTime(in DateTime date)
        {
            var nomalizedYear = (date.Year - 1924) % 12;
            if (nomalizedYear < 0) nomalizedYear = 12 + nomalizedYear;
            return (ChineseZodiac)nomalizedYear;
        }

        private static int YearDifference(DateTime f, DateTime s)
        {
            var partialYears = f.Year - s.Year;
            var day = s switch
            {
                { Day: 29, Month: 2 } when !DateTime.IsLeapYear(f.Year) => 28,
                _ => s.Day
            };
            return partialYears + (new DateTime(f.Year, s.Month, day) > f ? -1 : 0);
        }

        public static BirthdayDate Parse(DateTime date)
        {
            var cur = DateTime.Now;
            if (cur < date)
                throw new NotYetBornException(date);
            if (YearDifference(cur, date) > 123)
                throw new TooOldException(date);
            return new(date);
        }

        public override string ToString()
        {
            return Date.ToString();
        }

        public int CompareTo(object? obj)
        {
            return Date.CompareTo((obj as BirthdayDate)?.Date);
        }
    }

    public class TooOldException : Exception
    {
        public DateTime Value { get; private set; }
        public TooOldException(DateTime value) : base($"Date is too far back") => Value = value;
    }

    public class NotYetBornException : Exception
    {
        public DateTime Value { get; private set; }
        public NotYetBornException(DateTime value) : base($"Date hasn't even been reached") => Value = value;
    }
}