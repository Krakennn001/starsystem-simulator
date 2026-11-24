using StarSystemSimulator.Entities.Planets;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Services
{
    public sealed class PlanetGenerator
    {
        private readonly Random _random;
        public int Seed { get; }
        
        public PlanetGenerator(int? seed = null)
        {
            Seed = seed ?? Random.Shared.Next();
            _random = new Random(Seed);
        }

        public List<Planet> Generate(GenerationProbabilities probabilities, List<Satellite>? satellites = null, string? starName = null, int? amount = null)
        {
            List<Planet> planets = [];

            amount = amount is null ? _random.NextAuto(probabilities.Planets.Amount) : amount;

            for(int i = 0; i < amount; i++)
            {
                if (_random.NextDouble() < probabilities.Planets.EarthType)
                    planets.Add(GenerateEarthType(satellites, starName, i));
                else
                    planets.Add(GenerateGasGiant(satellites, starName, i));
            }

            return planets;
        }

        public EarthTypePlanet GenerateEarthType(List<Satellite>? satellites = null, string? starName = null, int orbitIndex = 0)
        {
            var limits = LimitsConfig.Get<PlanetLimits>(StarObjectType.EarthPlanet);
            var nameGenerator = new NameGenerator(Seed);

            return new EarthTypePlanet(
                nameGenerator.GenerateName(StarObjectType.EarthPlanet, starName, orbitIndex, _random.Next(2) == 0),
                _random.NextAuto(limits.Radius),
                _random.NextAuto(limits.Weight),
                _random.NextAuto(limits.Distance),
                _random.NextAuto(limits.Year),
                satellites ?? []
            );
        }

        public GasGiant GenerateGasGiant(List<Satellite>? satellites = null, string? starName = null, int orbitIndex = 0)
        {
            var limits = LimitsConfig.Get<PlanetLimits>(StarObjectType.GiantPlanet);
            var nameGenerator = new NameGenerator(Seed);

            return new GasGiant(
                 nameGenerator.GenerateName(StarObjectType.GiantPlanet, starName, orbitIndex, _random.Next(2) == 0),
                _random.NextAuto(limits.Radius),
                _random.NextAuto(limits.Weight),
                _random.NextAuto(limits.Distance),
                _random.NextAuto(limits.Year),
                satellites ?? []
            );
        }
    }
}
