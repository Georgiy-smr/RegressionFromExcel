using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression.RegressionServices;

internal class TwoFactRegressionService : IRegression<DataTwoFact>
{
    private readonly global::Regression.Two_factor_regression.Interfaces.Services.IRegressionAnalysisService _serviceRegression;

    public TwoFactRegressionService()
    {
        _serviceRegression = new ApproximationService(new Solver(), new RowParser(), new DerivativeCalculator());
    }

    public IEnumerable<double> Calc(IPolynomialExpression? regressionData)
    {
        return _serviceRegression.GetValues(regressionData);
    }
}