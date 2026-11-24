using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace StarSystemSimulator.Entities.FreeObjects
{
    [JsonDerivedType(typeof(Comet), typeDiscriminator: "comet")]
    [JsonDerivedType(typeof(Asteroid), typeDiscriminator: "asteroid")]
    public abstract class FreeObject : IDescriptable, IFreeObject
    {
        public required StarObjectType Type { get; set; }
        public required string Name { get; set; }
        public double Radius { get; set; }
        public double Weight { get; set; }
        public double Speed { get; set; }
        public double ToStarDistance { get; set; }

        protected FreeObject() { }

        [SetsRequiredMembers]
        protected FreeObject(StarObjectType objectType, string name,
            double radius, double weight, double speed, double toStarDistance)
        {
            var limits = LimitsConfig.Get<FreeObjectLimits>(objectType);

            Type = objectType;
            Name = name;
            Radius = radius.ValidateRange(limits.Radius, "Радиус");
            Weight = weight.ValidateRange(limits.Weight, "Масса");
            Speed = speed.ValidateRange(limits.Speed, "Скорость");
            ToStarDistance = toStarDistance.ValidateRange(limits.Distance, "Расстояние до звезды");
        }

        public virtual List<(string Text, Format Format, Color Color)> GetFullInfo() =>
        [
            ($"{Type.ToRusString(true)} {Name}\n", Format.Title, Color.Default),
            ($"Расстояние до звезды: {ToStarDistance:E2}\n", Format.PointedList, Color.Default),
            ($"Масса: {Weight:E2}кг\n", Format.PointedList, Color.Default),
            ($"Скорость: {Speed:N1}км/c\n", Format.PointedList, Color.Default),
            ($"Радиус: {Radius:N1}км\n", Format.PointedList, Color.Default),
        ];

        public virtual List<(string Text, Format Format, Color Color)> GetBrief() =>
        [
            ($"{Type.ToRusString(true)} {Name} ({Speed:N2}км/с)", Format.Default, Color.Default)
        ];
    }
}
