namespace StarSystemSimulator.Settings
{
    public sealed record AppSettings
    {
        public GenerationProbabilities GenerationProbabilities { get; init; } = new();
        public Visualization Visualization { get; init; } = new();
    }
}
