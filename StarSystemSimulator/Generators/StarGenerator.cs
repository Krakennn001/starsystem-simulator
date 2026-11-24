using StarSystemSimulator.Entities.Planets;
using StarSystemSimulator.Entities.Stars;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Services
{
    public sealed class StarGenerator
    {
        private readonly Random _random;
        public int Seed { get; }

        public StarGenerator(int? seed = null)
        {
            Seed = seed ?? Random.Shared.Next();
            _random = new Random(Seed);
        }

        public List<Star> Generate(GenerationProbabilities probabilities, int? amount = null)
        {
            List<Star> stars = [];

            amount = amount is null ? _random.NextAuto(probabilities.Stars.Amount) : amount;

            for (int i = 0; i < amount; i++)
            {
                double roll = _random.NextDouble();
                double threshold = 0;

                if (roll < (threshold += probabilities.Stars.NeutronStar))
                    stars.Add(GenerateNeutron());

                else if (roll < (threshold += probabilities.Stars.RedGiant))
                    stars.Add(GenerateRedGiant());

                else if (roll < (threshold += probabilities.Stars.MainStar))
                   stars.Add(GenerateMain());

                else
                   stars.Add(GenerateDwarf());
            }

            return stars;
        }

        public NeutronStar GenerateNeutron()
        {
            var limits = LimitsConfig.Get<StarLimits>(StarObjectType.NeutronStar);
            var nameGenerator = new NameGenerator(Seed);

            return new NeutronStar(
                nameGenerator.GenerateName(StarObjectType.NeutronStar),
                _random.NextAuto(limits.Weight),
                _random.NextAuto(limits.Temperature),
                _random.NextAuto(limits.Radius)
            );
        }

        public WhiteDwarf GenerateDwarf()
        {
            var limits = LimitsConfig.Get<StarLimits>(StarObjectType.WhiteDwarfStar);
            var nameGenerator = new NameGenerator(Seed);

            return new WhiteDwarf(
                nameGenerator.GenerateName(StarObjectType.WhiteDwarfStar),
                _random.NextAuto(limits.Weight),
                _random.NextAuto(limits.Temperature),
                _random.NextAuto(limits.Radius)
            );
        }

        public RedGiant GenerateRedGiant()
        {
            var limits = LimitsConfig.Get<StarLimits>(StarObjectType.RedGiantStar);
            var nameGenerator = new NameGenerator(Seed);

            return new RedGiant(
                nameGenerator.GenerateName(StarObjectType.RedGiantStar),
                _random.NextAuto(limits.Weight),
                _random.NextAuto(limits.Temperature),
                _random.NextAuto(limits.Radius)
            );
        }

        public MainStar GenerateMain()
        {
            var limits = LimitsConfig.Get<StarLimits>(StarObjectType.MainStar);
            var nameGenerator = new NameGenerator(Seed);

            return new MainStar(
                nameGenerator.GenerateName(StarObjectType.MainStar),
                _random.NextAuto(limits.Weight),
                _random.NextAuto(limits.Temperature),
                _random.NextAuto(limits.Radius)
            );
        }
    }
}
