using StarSystemSimulator.Entities.Planets;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Services
{
    public sealed class SatelliteGenerator
    {
        private readonly Random _random;
        public int Seed { get; }

        public SatelliteGenerator(int? seed = null)
        {
            Seed = seed ?? Random.Shared.Next();
            _random = new Random(Seed);
        }

        public List<Satellite> Generate(GenerationProbabilities probabilities, string? planetName = null, int? amount = null)
        {
            var limits = LimitsConfig.Get<PlanetLimits>(StarObjectType.Satellite);
            var nameGenerator = new NameGenerator(Seed);
            List<Satellite> satellites = [];

            amount = amount is null ? _random.NextAuto(probabilities.Satellites.Amount) : amount;

            for (int i = 0; i < amount; i++)
            {
                satellites.Add(new Satellite
                (
                    nameGenerator.GenerateName(StarObjectType.Satellite, planetName, i, _random.Next(2) == 0),
                    _random.NextAuto(limits.Radius),
                    _random.NextAuto(limits.Weight),
                    _random.NextAuto(limits.Distance),
                    _random.NextAuto(limits.Year),
                    i
                ));
            }

            return satellites;
        }
    }
}
