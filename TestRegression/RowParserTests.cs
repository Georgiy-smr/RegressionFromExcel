using System.Globalization;
using MathNet.Symbolics;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;

namespace TestRegression
{
    public class RowParserTests

    {
        private readonly IRowsParser _parser = new RowParser();
        private readonly IDerivatives _derivatives = new DerivativeCalculator();
        public void GetRowInputResultTrue()
        {
            //Вычисление производных
            var a2 = SymbolicExpression.Variable("a2");
            var a1 = SymbolicExpression.Variable("a1");
            Dictionary<double, double> XtoY = new Dictionary<double, double>()
            {
                { 0, 0 }, { 1, 1 }, { 2, 3 }, { 5, 10 }
            };
            SymbolicExpression poly = 0;
            foreach (var item in XtoY)
            {
                var expr = item.Value + (a1 * item.Key + a2);
                poly += expr * expr;
            }
            var diffs = _derivatives.Calculate(
                new VariableExpression(poly, new List<SymbolicExpression> { a2, a1 }));
            //получаем эталоный массив
            var trueArray = GetArrayParseX(diffs.First()).ToArray();
            //получаем проверяемый массив
            var check = _parser.GetRowInput(diffs.First(), 2);

            Assert.True(trueArray.SequenceEqual(check));
        }

        private static IEnumerable<double> GetArrayParseX(string[] strings)
        {
            var a = strings.Where(x => !x.Equals("+"));
            var listAr = new double[2];
            int sign = 1;
            foreach (var stringExpration in a)
            {
                if (stringExpration.Equals("-"))
                {
                    sign = -1;
                    continue;
                }
                if (stringExpration.Contains("*a"))
                {
                    var str = stringExpration.Remove(0, stringExpration!.IndexOf("a", StringComparison.Ordinal) + 1);
                    int index = int.Parse(str);
                    var st1r = stringExpration.Substring(0, stringExpration!.IndexOf("*", StringComparison.Ordinal));
                    listAr[index - 1] = sign * double.Parse(st1r, CultureInfo.InvariantCulture);
                    sign = 1;
                }
            }
            return listAr;
        }
    }
}