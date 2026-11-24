using StarSystemSimulator.Entities.FreeObjects;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Services
{
    public sealed class FreeObjectGenerator
    {
        private readonly Random _random;
        public int Seed { get; }

        public FreeObjectGenerator(int? seed = null)
        {
            Seed = seed ?? Random.Shared.Next();
            _random = new Random(Seed);
        }

        public List<FreeObject> Generate(GenerationProbabilities probabilities, int? amount = null)
        {
            List<FreeObject> freeObjects = [];

            amount = amount is null ? _random.NextAuto(probabilities.FreeObjects.Amount) : amount;

            for (int i = 0; i < amount; i++)
            {
                if (_random.NextDouble() < probabilities.FreeObjects.Asteroid)
                    freeObjects.Add(GenerateAsteroid());
                else
                    freeObjects.Add(GenerateComet());
            }

            return freeObjects;
        }

        public Asteroid GenerateAsteroid()
        {
            var limits = LimitsConfig.Get<FreeObjectLimits>(StarObjectType.Asteroid);
            var nameGenerator = new NameGenerator(Seed);

            return new Asteroid
                (
                    nameGenerator.GenerateName(StarObjectType.Asteroid),
                    _random.NextAuto(limits.Radius),
                    _random.NextAuto(limits.Weight),
                    _random.NextAuto(limits.Speed),
                    _random.NextAuto(limits.Distance)
                );
        }

        public Comet GenerateComet()
        {
            var limits = LimitsConfig.Get<FreeObjectLimits>(StarObjectType.Comet);
            var nameGenerator = new NameGenerator(Seed);

            return new Comet
                (
                    nameGenerator.GenerateName(StarObjectType.Comet),
                    _random.NextAuto(limits.Radius),
                    _random.NextAuto(limits.Weight),
                    _random.NextAuto(limits.Speed),
                    _random.NextAuto(limits.Distance),
                    _random.NextAuto(limits.Tail)
                );
        }
    }
}
