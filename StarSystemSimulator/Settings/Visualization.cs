using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Settings
{
    public sealed record Visualization
    {
        public Color PlanetColor { get; init; } = Color.Default;
        public Color SatelliteColor { get; init; } = Color.Default;
        public Color FreeObjectColor { get; init; } = Color.Default;

        public Range<int> StarsAmount { get; init; } = new(0, 2);
        public Range<int> PlanetsAmount { get; init; } = new(0, 10);
        public Range<int> FreeObjectsAmount { get; init; } = new(0, 50);

        public (int x, int y) Window { get; init; } = (200, 40);
        public (int x, int y) StarCenter { get; init; } = (30, 19);
        public (int x, int y) WHConstant { get; init; } = (8, 16);

        public char[] BrightSymbols { get; init; } = ['@', '&', '$', 'O', '#', '%', '*', '=', '+', '~', '-', ',', '.'];
    }
}
