using StarSystemSimulator.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace StarSystemSimulator.Settings
{
    public sealed record GenerationProbabilities
    {
        public StarProbabilities Stars { get; init; } = new();
        public PlanetProbabilities Planets { get; init; } = new();
        public SatelliteProbabilities Satellites { get; init; } = new();
        public FreeObjectProbabilities FreeObjects { get; init; } = new();
    }

    public sealed record StarProbabilities
    {
        public Range<int> Amount { get; set; } = new(1, 3);

        private double _neutron = 0.05;
        private double _redGiant = 0.15;
        private double _whiteDwarf = 0.20;
        private double _mainStar = 0.60;

        public double NeutronStar { get => _neutron; set => _neutron = ValidateProb(value); }
        public double RedGiant { get => _redGiant; set => _redGiant = ValidateProb(value); }
        public double WhiteDwarf { get => _whiteDwarf; set => _whiteDwarf = ValidateProb(value); }
        public double MainStar { get => _mainStar; set => _mainStar = ValidateProb(value); }

        public bool IsValidSum() => Math.Abs(NeutronStar + RedGiant + WhiteDwarf + MainStar - 1.0) < 1e-6;
        public void ValidateSum()
        {
            var sum = _neutron + _redGiant + _whiteDwarf + _mainStar;
            if (sum <= 1e-9)
            {
                _neutron = _redGiant = _whiteDwarf = _mainStar = 0.25;
                return;
            }

            var koef = sum / 1.0;
            _neutron /= koef;
            _redGiant /= koef;
            _whiteDwarf /= koef;
            _mainStar /= koef;
        }

        private static double ValidateProb(double v, [CallerMemberName] string name = "")
            => v.ValidateRange(0.0, 1.0, name);
    }

    public sealed record PlanetProbabilities
    {
        public Range<int> Amount { get; set; } = new(1, 10);

        private double _gasGiant = 0.3;

        public double GasGiant { get => _gasGiant; set => _gasGiant = ValidateProb(value); }
        [JsonIgnore]
        public double EarthType => 1.0 - GasGiant;

        private static double ValidateProb(double v, [CallerMemberName] string name = "")
            => v.ValidateRange(0.0, 1.0, name);
    }

    public sealed record SatelliteProbabilities
    {
        public Range<int> Amount { get; set; } = new(0, 15);
    }

    public sealed record FreeObjectProbabilities
    {
        public Range<int> Amount { get; set; } = new(1, 500);

        private double _comet = 0.5;

        public double Comet { get => _comet; set => _comet = ValidateProb(value); }
        [JsonIgnore]
        public double Asteroid => 1.0 - Comet;

        private static double ValidateProb(double v, [CallerMemberName] string name = "")
            => v.ValidateRange(0.0, 1.0, name);
    }
}
