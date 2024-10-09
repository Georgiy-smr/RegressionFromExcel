using Regression.Two_factor_regression;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

public interface IReadData<out T, in TEnum> where TEnum : Enum
{
    IAsyncEnumerable<T> ReadAsync(string pathReadingFile, TEnum? additional = default);
}