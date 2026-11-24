using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Descriptions
{
    public interface IDescriptable
    {
        StarObjectType Type { get; }
        string Name { get; }

        List<(string Text, Format Format, Color Color)> GetFullInfo();
        List<(string Text, Format Format, Color Color)> GetBrief();
    }
}
