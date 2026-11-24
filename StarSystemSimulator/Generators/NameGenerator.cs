using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Services
{
    public sealed class NameGenerator
    {
        private readonly Random _random;
        public int Seed { get; }

        private static readonly string[] Catalogs = ["Kepler", "TRAPPIST", "Gliese", "HD", "HIP", "TOI", "PSR", "Ross", "Wolf"];
        private static readonly string[] GreekLetters = ["Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Sigma", "Omega"];
        private static readonly string[] Constellations = ["Centauri", "Orionis", "Cygni", "Lyrae", "Draconis", "Andromedae", "Pegasi", "Cassiopeiae", "Eridani", "Aquarii"];


        private static readonly string[] WordsStart = ["Aure", "Vesper", "Chron", "Hyper", "Nyx", "Eos", "Astra", "Thal", "Kryo", "Heli", "Zeph", "Sol"];
        private static readonly string[] WordsEnd = ["ia", "on", "is", "os", "ea", "or", "us", "ar", "ix", "a"];

        public NameGenerator(int? seed = null)
        {
            Seed = seed ?? Random.Shared.Next();
            _random = new Random(Seed);
        }

        public string GenerateName(
            StarObjectType objectType,
            string? parentName = null,
            int orbitIndex = 0,
            bool useMythological = false)
        {
            return objectType switch
            {
                // Звезды
                StarObjectType.NeutronStar or
                StarObjectType.WhiteDwarfStar or
                StarObjectType.RedGiantStar or
                StarObjectType.MainStar => GenerateStarName(objectType),

                // Планеты
                StarObjectType.EarthPlanet or
                StarObjectType.GiantPlanet => GeneratePlanetName(
                    parentName ?? $"{Pick(Catalogs)}-{_random.Next(10, 9999)}",
                    orbitIndex),

                // Спутники
                StarObjectType.Satellite => GenerateSatelliteName(
                    parentName ?? $"{Pick(WordsStart)}{Pick(WordsEnd)}",
                    orbitIndex,
                    useMythological),

                // Свободные объекты
                StarObjectType.Asteroid => GenerateAsteroidName(),
                StarObjectType.Comet => GenerateCometName(),

                // По умолчанию
                _ => useMythological
                    ? $"{Pick(WordsStart)}{Pick(WordsEnd)}"
                    : $"OBJ-{_random.Next(1000, 9999)}"
            };
        }

        private string GenerateStarName(StarObjectType starType) =>
            starType switch
            {
                StarObjectType.NeutronStar => $"PSR J{_random.Next(1000, 2400):D4}-{_random.Next(1000, 9999):D4}",

                StarObjectType.WhiteDwarfStar => $"WD {_random.Next(100, 999):D3}+{_random.Next(10, 99):D2}",

                StarObjectType.RedGiantStar => $"{Pick(GreekLetters)} {Pick(Constellations)}",

                _ => _random.Next(2) == 0
                    ? $"{Pick(Catalogs)}-{_random.Next(10, 9999)}"
                    : $"{Pick(GreekLetters)} {Pick(Constellations)}"
            };

        private static string GeneratePlanetName(string starName, int orbitIndex) =>
            $"{starName} {(char)('b' + orbitIndex)}";

        private string GenerateSatelliteName(string planetName, int moonIndex, bool useMythological = false) =>
            useMythological ? $"{Pick(WordsStart)}{Pick(WordsEnd)}" : $"{planetName} {ToRoman(moonIndex + 1)}";

        private string GenerateAsteroidName()
        {
            int year = _random.Next(1990, 2035);
            char c1 = (char)_random.Next('A', 'Z' + 1);
            char c2 = (char)_random.Next('A', 'Z' + 1);
            int num = _random.Next(1, 500);
            return $"{year} {c1}{c2}{num}";
        }

        private string GenerateCometName()
        {
            char periodType = _random.Next(2) == 0 ? 'C' : 'P';
            int year = _random.Next(1980, 2035);
            char halfMonth = (char)_random.Next('A', 'Z' + 1);
            int num = _random.Next(1, 15);
            return $"{periodType}/{year} {halfMonth}{num}";
        }

        private T Pick<T>(T[] array) => array[_random.Next(array.Length)];

        private static string ToRoman(int number) => number switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            4 => "IV",
            5 => "V",
            6 => "VI",
            7 => "VII",
            8 => "VIII",
            9 => "IX",
            10 => "X",
            _ => number.ToString()
        };
    }
}
