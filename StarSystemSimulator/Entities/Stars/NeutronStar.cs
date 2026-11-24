using StarSystemSimulator.Services;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities.Stars
{
    public sealed class NeutronStar : Star
    {
        public NeutronStar() { }

        [SetsRequiredMembers]
        public NeutronStar(string name, double weight, int temperature, double radius)
            : base(StarObjectType.NeutronStar,
                name, weight, temperature, radius)
        {
            (GlowColor, _) = StarUtils.GetSpectralData(temperature);
        }
    }
}
