using MathNet.Symbolics;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRegression
{
    public class DerivativeCalculatorTests
    {
        private readonly IDerivatives _derivatives = new DerivativeCalculator();
        [Fact]
        public void CalcLinearPolinomialResultTrue()
        {
            //Вычисление частной производной dpdk
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
            var dpdk = poly.Differentiate(a2).RationalSimplify(a2).ToString().Trim().Split();
            //вычисление производной через сервис

            var tested = _derivatives.Calculate(
                new VariableExpression(poly, new List<SymbolicExpression> { a2, a1 }));

            Assert.True(dpdk.SequenceEqual(tested.First()));


        }
    }
}
