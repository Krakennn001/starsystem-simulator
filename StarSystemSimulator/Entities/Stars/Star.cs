using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace StarSystemSimulator.Entities.Stars
{
    [JsonDerivedType(typeof(MainStar), typeDiscriminator: "main")]
    [JsonDerivedType(typeof(WhiteDwarf), typeDiscriminator: "white_dwarf")]
    [JsonDerivedType(typeof(RedGiant), typeDiscriminator: "red_giant")]
    [JsonDerivedType(typeof(NeutronStar), typeDiscriminator: "neutron")]
    public abstract class Star : IDescriptable, IStar
    {
        public required StarObjectType Type { get; set; }
        public required string Name { get; set; }
        public Color GlowColor { get; set; }
        public double Weight { get; set; }
        public int Temperature { get; set; }
        public double Radius { get; set; }

        protected Star() { }

        [SetsRequiredMembers]
        protected Star(StarObjectType objectType, string name, double weight, int temperature, double radius)
        {
            var limits = LimitsConfig.Get<StarLimits>(objectType);

            Type        = objectType;
            Name        = name;
            Weight      = weight.ValidateRange(limits.Weight, "Масса");
            Temperature = temperature.ValidateRange(limits.Temperature, "Температура");
            Radius      = radius.ValidateRange(limits.Radius, "Радиус");
        }

        public virtual List<(string Text, Format Format, Color Color)> GetFullInfo() =>
        [
            ($"{Type.ToRusString(true)} {Name}\n", Format.Title, GlowColor),
            ("Свечение: ", Format.PointedList, Color.Default),
            ($"{GlowColor.ToRusString()}\n", Format.Default, GlowColor),
            ($"Масса: {Weight:E2} кг\n", Format.PointedList, Color.Default),
            ($"Температура: {Temperature:N0} К\n", Format.PointedList, Color.Default),
            ($"Радиус: {Radius:N1} км\n", Format.PointedList, Color.Default)
        ];

        public virtual List<(string Text, Format Format, Color Color)> GetBrief() =>
        [
            ($"{Type.ToRusString(true)} {Name}", Format.Default, GlowColor)
        ];
    }
}
