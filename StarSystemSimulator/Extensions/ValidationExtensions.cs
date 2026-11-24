using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;
using StarSystemSimulator.Services;

namespace StarSystemSimulator.Extensions
{
    public static class ValidationExtensions
    {
        public static T ValidateRange<T>(this T value, T min, T max, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
                throw new OutOfRangeException<T>(paramName, min, max, value);
            return value;
        }

        public static T ValidateRange<T>(this T value, Range<T> range, string paramName) where T : IComparable<T>, IParsable<T>
            => ValidateRange(value, range.Min, range.Max, paramName);

        public static bool IsInRange<T>(this T value, T min, T max) where T : IComparable<T>
            => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;

        public static bool IsInRange<T>(this T value, Range<T> range) where T : IComparable<T>, IParsable<T>
            => IsInRange(value, range.Min, range.Max);

        public static (string, Format, Color) ToDefault(this string value) => (value, Format.Default, Color.Default);
    }
}
