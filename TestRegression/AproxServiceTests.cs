using MathNet.Symbolics;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces.Services;
using Regression.Two_factor_regression.Interfaces;
using Regression.Two_factor_regression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRegression
{
    public class AproxServiceTests
    {
        private IRegressionAnalysisService _testService;
       

        [Fact]
        public void CalcTwoFactorCoefs()
        {
            var a0 = SymbolicExpression.Variable("a0");
            var a1 = SymbolicExpression.Variable("a1");
            var a2 = SymbolicExpression.Variable("a2");
            var a3 = SymbolicExpression.Variable("a3");
            var a4 = SymbolicExpression.Variable("a4");
            var a5 = SymbolicExpression.Variable("a5");
            var a6 = SymbolicExpression.Variable("a6");
            var a7 = SymbolicExpression.Variable("a7");
            var a8 = SymbolicExpression.Variable("a8");
            var a9 = SymbolicExpression.Variable("a9");
            var a10 = SymbolicExpression.Variable("a10");
            var a11 = SymbolicExpression.Variable("a11");
            var a12 = SymbolicExpression.Variable("a12");
            var a13 = SymbolicExpression.Variable("a13");
            var a14 = SymbolicExpression.Variable("a14");
            var a15 = SymbolicExpression.Variable("a15");

            IEnumerable<DataTwoFact> X1X2toY = GetRandomToFactData(20);
            
            SymbolicExpression poly = 0;
            foreach (var item in X1X2toY)
            {
                var expr = item.Y +
                           (a0 +
                            a1 * item.X2 +
                            a2 * item.X2 * item.X2 +
                            a3 * item.X1 +
                            a4 * item.X1 * item.X1 +
                            a5 * item.X1 * item.X2 +
                            a6 * item.X2 * item.X1 * item.X1 +
                            a7 * item.X2 * item.X2 * item.X1 +
                            a8 * item.X1 * item.X1 * item.X2 * item.X2 +
                            a9 * item.X1 * item.X1 * item.X1 +
                            a10 * item.X2 * item.X1 * item.X1 * item.X1 +
                            a11 * item.X2 * item.X2 * item.X1 * item.X1 * item.X1 +
                            a12 * item.X2 * item.X2 * item.X2 +
                            a13 * item.X2 * item.X2 * item.X2 * item.X1 +
                            a14 * item.X2 * item.X2 * item.X2 * item.X1 * item.X1 +
                            a15 * item.X2 * item.X2 * item.X2 * item.X1 * item.X1 * item.X1);
                poly += expr * expr;
            }

            _testService = new ApproximationService(new Solver(),
                    new RowParser(), new DerivativeCalculator());
            //инициализация данных
            IPolynomialExpression polinomial = new VariableExpression(poly, new List<SymbolicExpression>
                   {a0,a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15});
            //получение данных
            var resultCheckedBytes = _testService.GetValues(polinomial).ToArray();
            double res = 0;
            var data = X1X2toY.Last();
            for (int i = 0; i < resultCheckedBytes.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        res += resultCheckedBytes[i];
                        break;
                    case 1:
                        //a1* item.X2
                        res += resultCheckedBytes[i] * data.X2;
                        break;
                    case 2:
                        //a2* item.X2* item.X2 +
                        res += resultCheckedBytes[i] * data.X2 * data.X2;
                        break;
                    case 3:
                        //a3* item.X1 +
                        res += resultCheckedBytes[i] * data.X1;
                        break;
                    case 4:
                        //   a4 * item.X1 * item.X1 +
                        res += resultCheckedBytes[i] * data.X1 * data.X1;
                        break;
                    case 5:
                        //a5 * item.X1 * item.X2 +
                        res += resultCheckedBytes[i] * data.X1 * data.X2;
                        break;
                    case 6:
                        // a6* item.X2* item.X1* item.X1 +
                        res += resultCheckedBytes[i] * data.X2 * data.X1 * data.X1;
                        break;
                    case 7:
                        //a7* item.X2* item.X2* item.X1 +
                        res += resultCheckedBytes[i] * data.X2 * data.X2 * data.X1;
                        break;
                    case 8:
                        //a8* item.X1* item.X1* item.X2* item.X2 +
                        res += resultCheckedBytes[i] * data.X1 * data.X1 * data.X2 * data.X2;
                        break;
                    case 9:
                        //a9* item.X1* item.X1* item.X1 +
                        res += resultCheckedBytes[i] * data.X1 * data.X1 * data.X1;
                        break;
                    case 10:
                        //a10* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedBytes[i] * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 11:
                        //a11* item.X2* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedBytes[i] * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 12:
                        //a12* item.X2* item.X2* item.X2 +
                        res += resultCheckedBytes[i] * data.X2 * data.X2 * data.X2;
                        break;
                    case 13:
                        //a13* item.X2* item.X2* item.X2* item.X1 +
                        res += resultCheckedBytes[i] * data.X2 * data.X2 * data.X2 * data.X1;
                        break;
                    case 14:
                        //a14* item.X2* item.X2* item.X2* item.X1* item.X1 +
                        res += resultCheckedBytes[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1;
                        break;
                    case 15:
                        //a15* item.X2* item.X2* item.X2* item.X1* item.X1* item.X1);
                        res += resultCheckedBytes[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                }
            }

            Assert.True(resultCheckedBytes.Any());

        }


        [Fact]
        public void TestExtensionMethodReturnIPolynomialExpression()
        {

            IPolynomialExpression testedExpression = GetRandomToFactData(20)
                .CreateThirdOrderPolynomialExpression();


            _testService = new ApproximationService(new Solver(), new RowParser(), new DerivativeCalculator());
            var resultCheckedBytes = _testService.GetValues(testedExpression).ToArray();
            Assert.True(resultCheckedBytes.Any());
        }

        private IEnumerable<DataTwoFact> GetRandomToFactData(int countData)
        {
            var rand = new Random();
            for (int i = 0; i < countData; i++)
            {
                yield return new DataTwoFact()
                {
                    X1 = i,
                    X2 = rand.Next(10),
                    Y = i + rand.NextDouble() / 100
                };
            }
        }

        
    }
}
