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
        private IRegressionAnalysisService sut;
       

        [Fact]
        public void Error_calculating_random_data_less_then_one_percent()
        {
            //arrange
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

            IEnumerable<DataTwoFact> randomDataX1X2toY = GetRandomToFactData(20);
            
            SymbolicExpression poly = 0;
            foreach (var item in randomDataX1X2toY)
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

            sut = new ApproximationService(new Solver(),
                    new RowParser(), new DerivativeCalculator());
            //инициализация данных
            IPolynomialExpression polinomial = new VariableExpression(poly, new List<SymbolicExpression>
                   {a0,a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15});
            //act
            double[] pollynominalCoefficients = sut.GetValues(polinomial).ToArray();
            double testedResult = 0;
            var expected = randomDataX1X2toY.Last();
            for (int i = 0; i < pollynominalCoefficients.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        testedResult += pollynominalCoefficients[i];
                        break;
                    case 1:
                        //a1* item.X2
                        testedResult += pollynominalCoefficients[i] * expected.X2;
                        break;
                    case 2:
                        //a2* item.X2* item.X2 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X2;
                        break;
                    case 3:
                        //a3* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X1;
                        break;
                    case 4:
                        //   a4 * item.X1 * item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X1 * expected.X1;
                        break;
                    case 5:
                        //a5 * item.X1 * item.X2 +
                        testedResult += pollynominalCoefficients[i] * expected.X1 * expected.X2;
                        break;
                    case 6:
                        // a6* item.X2* item.X1* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X1 * expected.X1;
                        break;
                    case 7:
                        //a7* item.X2* item.X2* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X2 * expected.X1;
                        break;
                    case 8:
                        //a8* item.X1* item.X1* item.X2* item.X2 +
                        testedResult += pollynominalCoefficients[i] * expected.X1 * expected.X1 * expected.X2 * expected.X2;
                        break;
                    case 9:
                        //a9* item.X1* item.X1* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X1 * expected.X1 * expected.X1;
                        break;
                    case 10:
                        //a10* item.X2* item.X1* item.X1* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X1 * expected.X1 * expected.X1;
                        break;
                    case 11:
                        //a11* item.X2* item.X2* item.X1* item.X1* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X2 * expected.X1 * expected.X1 * expected.X1;
                        break;
                    case 12:
                        //a12* item.X2* item.X2* item.X2 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X2 * expected.X2;
                        break;
                    case 13:
                        //a13* item.X2* item.X2* item.X2* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X2 * expected.X2 * expected.X1;
                        break;
                    case 14:
                        //a14* item.X2* item.X2* item.X2* item.X1* item.X1 +
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X2 * expected.X2 * expected.X1 * expected.X1;
                        break;
                    case 15:
                        //a15* item.X2* item.X2* item.X2* item.X1* item.X1* item.X1);
                        testedResult += pollynominalCoefficients[i] * expected.X2 * expected.X2 * expected.X2 * expected.X1 * expected.X1 * expected.X1;
                        break;
                }
            }

            var err = 100 * Math.Abs(expected.Y - testedResult) / expected.Y;
            Assert.True(err < 1);
        }


        [Fact]
        public void TestExtensionMethodReturnIPolynomialExpression()
        {

            IPolynomialExpression testedExpression = GetRandomToFactData(20)
                .CreateThirdOrderPolynomialExpression();


            sut = new ApproximationService(new Solver(), new RowParser(), new DerivativeCalculator());
            var resultCheckedBytes = sut.GetValues(testedExpression).ToArray();
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
