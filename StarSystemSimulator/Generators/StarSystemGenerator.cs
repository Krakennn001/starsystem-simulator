using StarSystemSimulator.Entities;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Services
{
    public sealed class StarSystemGenerator
    {
        private readonly Random _random;
        public int Seed { get; }

        public StarSystemGenerator(int? seed = null)
        {
            Seed = seed ?? Random.Shared.Next();
            _random = new Random(Seed);
        }

        public StarSystem Generate(GenerationProbabilities probabilities)
        {
            var starGenerator = new StarGenerator(_random.Next());
            var planetGenerator = new PlanetGenerator(_random.Next());
            var satelliteGenerator = new SatelliteGenerator(_random.Next());
            var freeObjectGenerator = new FreeObjectGenerator(_random.Next());
            var nameGenerator = new NameGenerator(_random.Next());

            var stars = starGenerator.Generate(probabilities);
            var planets = planetGenerator.Generate(probabilities, null, stars[0].Name);
            foreach (var planet in planets)
            {
                var satellites = satelliteGenerator.Generate(probabilities, planet.Name);
                planet.Satellites = satellites;
            }
            var freeObjects = freeObjectGenerator.Generate(probabilities);

            return new StarSystem(nameGenerator.GenerateName(StarObjectType.StarSystem), stars, planets, freeObjects);
        }
    }
}
