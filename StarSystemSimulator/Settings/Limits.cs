using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Settings
{
    public readonly record struct Range<T>(T Min, T Max) : IParsable<Range<T>>
    where T : IParsable<T>, IComparable<T>
    {
        public override string ToString() => $"{Min} : {Max}";

        public static bool TryParse([NotNullWhen(true)] string? s, out Range<T> result)
        {
            return TryParse(s, provider: null, out result);
        }

        public static bool TryParse(
            [NotNullWhen(true)] string? s,
            IFormatProvider? provider,
            out Range<T> result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            var parts = s.Trim().Trim('[', ']').Split(':', StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
                return false;

            if (T.TryParse(parts[0], provider, out T? min) &&
                T.TryParse(parts[1], provider, out T? max))
            {
                if (min.CompareTo(max) > 0)
                    return false;

                result = new Range<T>(min, max);
                return true;
            }

            return false;
        }

        // Второй обязательный метод интерфейса IParsable<T>
        public static Range<T> Parse(string s, IFormatProvider? provider = null)
        {
            if (TryParse(s, provider, out var result))
                return result;

            throw new FormatException($"Не удалось преобразовать '{s}' в диапазон Range<{typeof(T).Name}>.");
        }
    }

    public abstract record Limits(
        Range<double> Weight,
        Range<double> Radius
    );

    public record StarLimits(
        Range<double> Weight,
        Range<double> Radius,
        Range<int> Temperature
    ) : Limits(Weight, Radius);

    public record PlanetLimits(
        Range<double> Weight,
        Range<double> Radius,
        Range<double> Year,
        Range<double> Distance
    ) : Limits(Weight, Radius);

    public record FreeObjectLimits(
        Range<double> Weight,
        Range<double> Radius,
        Range<double> Speed,
        Range<double> Distance,
        Range<double> Tail,
        Range<double> Activation
    ) : Limits(Weight, Radius);

    public static class LimitsConfig
    {
        public const double SUN_WEIGHT = 1.989e30; // в кг
        public const double SPEED_OF_LIGHT = 300_000;  // в км/с
        public const double SUN_RADIUS = 696_340;  // в км
        public const double EARTH_RADIUS = 6_378;    // в км
        public const double EARTH_WEIGHT = 5.972e24; // в кг
        public const double A_E = 1.496e8;  // в км

        public static readonly Dictionary<StarObjectType, Limits> Limits = new()
        {
            [StarObjectType.MainStar] = new StarLimits(
                Weight: new(0.05 * SUN_WEIGHT, 50 * SUN_WEIGHT),
                Radius: new(0.1 * SUN_RADIUS, 10 * SUN_RADIUS),
                Temperature: new(3_700, 30_000)
            ),
            [StarObjectType.RedGiantStar] = new StarLimits(
                Weight: new(50 * SUN_WEIGHT, 120 * SUN_WEIGHT),
                Temperature: new(30_000, 50_000),
                Radius: new(10 * SUN_RADIUS, 500 * SUN_RADIUS)
            ),
            [StarObjectType.WhiteDwarfStar] = new StarLimits(
                Weight: new(0.005 * SUN_WEIGHT, 0.1 * SUN_WEIGHT),
                Temperature: new(2_400, 3_700),
                Radius: new(0.001 * SUN_RADIUS, 0.1 * SUN_RADIUS)
            ),
            [StarObjectType.NeutronStar] = new StarLimits(
                Weight: new(5 * SUN_WEIGHT, 20 * SUN_WEIGHT),
                Temperature: new(500_000, 1_000_000),
                Radius: new(5, 20)
            ),
            [StarObjectType.EarthPlanet] = new PlanetLimits(
                Year: new(10, 5_000),
                Radius: new(0.1 * EARTH_RADIUS, 4 * EARTH_RADIUS),
                Weight: new(0.005 * EARTH_WEIGHT, 10 * EARTH_WEIGHT),
                Distance: new(0.1 * A_E, 4 * A_E)
            ),
            [StarObjectType.GiantPlanet] = new PlanetLimits(
                Year: new(50, 200_000),
                Radius: new(1 * EARTH_RADIUS, 50 * EARTH_RADIUS),
                Weight: new(1 * EARTH_WEIGHT, 1e3 * EARTH_WEIGHT),
                Distance: new(1.5 * A_E, 100 * A_E)
            ),
            [StarObjectType.Satellite] = new PlanetLimits(
                Year: new(0.1, 1_000),
                Radius: new(1, 1e4),
                Weight: new(10e10, 10 * EARTH_WEIGHT),
                Distance: new(1e-4 * A_E, 0.1 * A_E)
            ),
            [StarObjectType.Asteroid] = new FreeObjectLimits(
                Speed: new(0.1, 100),
                Weight: new(1e15, 1e20),
                Radius: new(0.1, 500),
                Distance: new(0.01 * A_E, 200 * A_E),
                Tail: new(0, 0),
                Activation: new(0, 0)
            ),
            [StarObjectType.Comet] = new FreeObjectLimits(
                Speed: new(0.1, 100),
                Radius: new(0.1, 500),
                Weight: new(1e15, 1e20),
                Distance: new(0.05 * A_E, 400 * A_E),
                Tail: new(1e3, 1e6),
                Activation: new(2 * A_E, double.MaxValue)
            )
        };

        public static T Get<T>(StarObjectType type) where T : Limits
            => (T)Limits[type];
    }
}
