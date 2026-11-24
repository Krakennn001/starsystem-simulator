using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Services;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities.Stars
{
    public sealed class RedGiant : Star
    {
        public SpectralClass Class { get; set; }

        public RedGiant() { }

        [SetsRequiredMembers]
        public RedGiant(string name, double weight, int temperature, double radius)
            : base(StarObjectType.RedGiantStar,
                name, weight, temperature, radius)
        {
            (GlowColor, Class) = StarUtils.GetSpectralData(temperature);
        }

        public override List<(string, Format, Color)> GetFullInfo()
        {
            var res = base.GetFullInfo();
            res.Add(("Спектральный класс: ", Format.PointedList, Color.Default));
            res.Add(($"{Class}\n", Format.Default, GlowColor));
            return res;
        }

        public override List<(string, Format, Color)> GetBrief()
        {
            var res = base.GetBrief();
            var first = res[0];
            res[0] = ($"{first.Text} ({Class})", first.Format, first.Color);
            return res;
        }
    }
}
