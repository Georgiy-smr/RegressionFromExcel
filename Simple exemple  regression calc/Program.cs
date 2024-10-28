using MathNet.Symbolics;
using System.Globalization;

namespace Simple_exemple_regression_calc
{
    internal class Program
    {
        static void Main()
        {
            //Символьное выражение членов полинома a1 * x + a2 
            var a1 = SymbolicExpression.Variable("a1");
            var a2 = SymbolicExpression.Variable("a2");

            //Аппроксимируемая выборка значений пара X Y
            Dictionary<double, double> XtoY = new()
            {
                { 0, 0 }, { 1, 1 }, { 2, 3 }, { 5, 10 }
            };

            //Построение функции F(x) 
            SymbolicExpression Fx = 0;
            foreach (var item in XtoY)
            {
                var expr = item.Value + (a1 * item.Key + a2);
                Fx += expr * expr;
            }

            //Частные производные в строковом виде
            var diffA1 = Fx.Differentiate(a1).RationalSimplify(a1).ToString().Trim().Split();
            var diffA2 = Fx.Differentiate(a2).RationalSimplify(a2).ToString().Trim().Split();

            List<string[]> polysList = new() { diffA1, diffA2 };
            // Step 1 Построение матрицы Y
            double[,] Y = new double[2, 1];
            for (int i = 0; i < Y.GetUpperBound(0) + 1; i++)
            {
                var yValues = GetArrayParseY(polysList[i]).ToArray();
                for (int j = 0; j < Y.GetUpperBound(1) + 1; j++)
                {
                    Y[i, 0] += yValues[j];
                }
            }

            // Step 2 Построение матрицы X
            double[,] X = new double[2, 2];
            for (int i = 0; i < X.GetUpperBound(0) + 1; i++)
            {
                var xValues = GetArrayParseX(polysList[i]).ToArray();
                for (int j = 0; j < X.GetUpperBound(1) + 1; j++)
                {
                    X[i, j] += xValues[j];
                }
            }
            // Step 3 Решение системы уравнений
            foreach (var root in (new Gauss()).Roots(X, Y))
                Console.WriteLine(root);

        }
        /// <summary>
        /// Вернуть из строки массив X значений
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private static IEnumerable<double> GetArrayParseX(string[] strings)
        {
            var a = strings.Where(x => !x.Equals("+"));
            var listAr = new double[2];
            int sign = 1;
            foreach (var stringExpression in a)
            {
                if (stringExpression.Equals("-"))
                {
                    sign = -1;
                    continue;
                }

                if (!stringExpression.Contains("*a")) continue;
                var str = stringExpression.Remove(0, stringExpression!.IndexOf("a", StringComparison.Ordinal) + 1);
                var index = int.Parse(str);
                var st1r = stringExpression.Substring(0, stringExpression!.IndexOf("*", StringComparison.Ordinal));
                listAr[index - 1] = sign * double.Parse(st1r, CultureInfo.InvariantCulture);
                sign = 1;
            }
            return listAr;
        }
        /// <summary>
        /// Вернуть из строки массив Y значений
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private static IEnumerable<double> GetArrayParseY(string[] strings)
        {
            var a = strings.Where(x => !x.Equals("+"));
            var sign = 1;
            foreach (var stringExpression in a)
            {
                if (stringExpression.Equals("-"))
                {
                    sign = -1;
                    continue;
                }

                if (stringExpression.Contains("*a")) continue;
                yield return sign * double.Parse(stringExpression, CultureInfo.InvariantCulture);
                sign = 1;
            }
        }
    }
}
