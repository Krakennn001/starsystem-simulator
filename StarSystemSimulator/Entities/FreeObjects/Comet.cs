using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities.FreeObjects
{
    public sealed class Comet : FreeObject
    {
        public double TailLength { get; set; }
        public bool IsActive { get; set; }

        public Comet() { }

        [SetsRequiredMembers]
        public Comet(string name, double radius, double weight, double speed, double distance, double tailLength)
            : base(StarObjectType.Comet,
                  name, radius, weight, speed, distance)
        {
            var limit = LimitsConfig.Get<FreeObjectLimits>(StarObjectType.Comet);

            TailLength = tailLength.ValidateRange(limit.Tail, "Длина хвоста");
            IsActive = distance.IsInRange(limit.Activation);
        }

        public override List<(string, Format, Color)> GetFullInfo()
        {
            var res = base.GetFullInfo();
            var first = res[0];
            res[0] = ($"{first.Text} ({(IsActive ? "Активна" : "Неактивна")})", first.Format, first.Color);
            res.Add(($"Длина хвоста: {TailLength}\n", Format.PointedList, Color.Default));
            return res;
        }

        public override List<(string, Format, Color)> GetBrief()
        {
            var res = base.GetBrief();
            var first = res[0];
            res[0] = ($"{first.Text} ({(IsActive ? "Активна" : "Неактивна")})", first.Format, first.Color);
            return res;
        }
    }
}
