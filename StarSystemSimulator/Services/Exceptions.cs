namespace StarSystemSimulator.Services
{
    public class OutOfRangeException<T> : ArgumentOutOfRangeException where T : IComparable<T>
    {
        public OutOfRangeException(string paramName, T min, T max, T actualValue)
        : base(paramName, actualValue, $"Значение {actualValue} должно быть в диапазоне от {min} до {max}.") { }
    }
}
