using StarSystemSimulator.Settings;

namespace StarSystemSimulator.Extensions
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Автоматически выбирает распределение для нецелочисленных диапазонов
        /// </summary>
        public static double NextAuto(this Random random, Range<double> range)
        {
            if (range.Min >= range.Max)
                return range.Min;

            if (range.Min <= 0)
            {
                double span = range.Max - range.Min;
                return span >= 100
                    ? random.NextSkewed(range, power: 2.0)
                    : random.NextLinear(range);
            }

            double ratio = range.Max / range.Min;

            return ratio switch
            {
                >= 100.0 => random.NextLog(range),
                >= 10.0 => random.NextSkewed(range, power: 2.5),
                >= 3.0 => random.NextSkewed(range, power: 1.6),
                _ => random.NextLinear(range)
            };
        }

        /// <summary>
        /// Автоматически выбирает распределение для целочисленных диапазонов
        /// </summary>
        public static int NextAuto(this Random random, Range<int> range)
        {
            if (range.Min >= range.Max)
                return range.Min;

            if (range.Min <= 0)
            {
                int span = range.Max - range.Min;
                return span >= 50
                    ? random.NextSkewed(range, power: 2.0)
                    : random.NextLinear(range);
            }

            double ratio = (double)range.Max / range.Min;

            return ratio switch
            {
                >= 100.0 => random.NextLog(range),
                >= 10.0 => random.NextSkewed(range, power: 2.5),
                >= 3.0 => random.NextSkewed(range, power: 1.6),
                _ => random.NextLinear(range)
            };
        }

        /// <summary>
        /// Случайное нецелое число через линейное распределение
        /// </summary>
        public static double NextLinear(this Random random, Range<double> range)
            => range.Min + random.NextDouble() * (range.Max - range.Min);

        /// <summary>
        /// Случайное целое число через линейное распределение
        /// </summary>
        public static int NextLinear(this Random random, Range<int> range)
            => random.Next(range.Min, range.Max + 1);

        /// <summary>
        /// Случайное нецелое число через квадратичное распределение
        /// </summary>
        public static double NextSkewed(this Random random, Range<double> range, double power = 2.5)
        {
            double t = Math.Pow(random.NextDouble(), power);
            return range.Min + t * (range.Max - range.Min);
        }

        /// <summary>
        /// Случайное целое число через квадратичное распределение
        /// </summary>
        public static int NextSkewed(this Random random, Range<int> range, double power = 2.5)
        {
            double t = Math.Pow(random.NextDouble(), power);
            int value = range.Min + (int)(t * (range.Max - range.Min + 1));
            return Math.Clamp(value, range.Min, range.Max);
        }

        /// <summary>
        /// Случайное нецелое число через логарифмическое распределение
        /// </summary>
        public static double NextLog(this Random random, Range<double> range)
        {
            if (range.Min <= 0)
                throw new ArgumentOutOfRangeException(nameof(range), "Min должен быть > 0 для логарифма.");

            double logMin = Math.Log(range.Min);
            double logMax = Math.Log(range.Max);
            double randomLog = logMin + random.NextDouble() * (logMax - logMin);

            return Math.Exp(randomLog);
        }

        /// <summary>
        /// Случайное целое число через логарифмическое распределение
        /// </summary>
        public static int NextLog(this Random random, Range<int> range)
        {
            if (range.Min <= 0)
                throw new ArgumentOutOfRangeException(nameof(range), "Min должен быть > 0 для логарифма.");

            double logMin = Math.Log(range.Min);
            double logMax = Math.Log(range.Max + 1);
            double randomLog = logMin + random.NextDouble() * (logMax - logMin);

            return Math.Clamp((int)Math.Exp(randomLog), range.Min, range.Max);
        }
    }
}
