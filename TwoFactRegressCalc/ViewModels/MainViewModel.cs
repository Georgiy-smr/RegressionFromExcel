using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Extansions.TwoFactExpression;
using TwoFactRegressCalc.Infrastructure.Commands.Base;
using TwoFactRegressCalc.Infrastructure.DI.Services.Creator;
using TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Writer;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels.Base;
using Microsoft.WindowsAPICodePack.Dialogs;
using Regression.Two_factor_regression.Interfaces;
using TwoFactRegressCalc.Enums.PolynomialDegrees;
using TwoFactRegressCalc.Infrastructure.DI.Services.JsonFileService;

namespace TwoFactRegressCalc.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        public MainViewModel(
            IReadData<DataTwoFact, Degree> dataExcelReader, 
            IDialogService dialog, 
            IRegression<DataTwoFact> regression,
            IWriteData<IEnumerable<double[]>> writer
        )
        {
            _dataExcelReader = dataExcelReader;
            _filedialog = dialog;
            _regression = regression;
            _writer = writer;
        }
        private readonly IReadData<DataTwoFact, Degree> _dataExcelReader;
        private readonly IDialogService _filedialog;
        private readonly IRegression<DataTwoFact> _regression;
        private readonly IWriteData<IEnumerable<double[]>> _writer;



        private Degree _degreeSelected;
        public Degree DegreeSelected
        {
            get => _degreeSelected;
            set => Set(ref _degreeSelected, value);
        }
        private List<Degree>? _degrees;
        public List<Degree> Degrees => _degrees ??= new (){ Degree.Three, Degree.Two };


        private string _title = "Two-factor regression";
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private Dictionary<Degree, Func<List<DataTwoFact>, IPolynomialExpression>> _dergreeExpression = new()
        {
            { Degree.Three, (data) => data.CreateThirdOrderPolynomialExpression()},
            { Degree.Two, (data) => data.CreateTwoOrderPolynomialExpression()},
        };

        private IPolynomialExpression? GetExpression(List<DataTwoFact> data)
        {
            if (!_dergreeExpression.TryGetValue(DegreeSelected, out Func<List<DataTwoFact>, IPolynomialExpression>? value))
                throw new NotSupportedException();
            return value.Invoke(data);
        }

        private int _minCount => DegreeSelected switch
        {
            Degree.Two => 8,
            Degree.Three => 15,
            _ =>  throw new NotSupportedException()
        };

        #region CalcFromExel Расчет из файла эксель

        private ICommand? _сalcFromExelCommand;


        public ICommand СalcFromExсelCommand =>
            _сalcFromExelCommand ?? new LambdaCommandAsync(OnCalcFromExelCommandExecuted, CanCalcFromExelCommandExecute);

        private async Task OnCalcFromExelCommandExecuted(object arg)
        {
            _filedialog.Filter = "Excel workbooks (*.xlsx)|*.xlsx";

            if (!_filedialog.OpenFileDialog()) 
                return;

            var dataRead = await _dataExcelReader.ReadAsync(_filedialog.FilePath).ToListAsync();
            if(dataRead.Count <= _minCount)
                return;

            var result = _regression.CalcCoefs(GetExpression(dataRead)).ToArray();
            
          
            if (result!.Any())
            {
                await _writer.Write(new List<double[]> { result }, _filedialog.FilePath);
                var check = DegreeSelected switch
                {
                    Degree.Two => dataRead.Select(x => CalcResTemp(x, result.ToArray())).Max(),
                    Degree.Three => dataRead.Select(x => CalcResDelta(x, result.ToArray())).Max(),
                    _ => throw new NotSupportedException()
                };
                MessageBox.Show($"Delta max = {check}");
            }
            else MessageBox.Show("Error.");

        }
        private double CalcResTemp(DataTwoFact data, double[] resultCheckedCoef)
        {
            double res = 0;
            for (int i = 0; i < resultCheckedCoef.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        res += resultCheckedCoef[i];
                        break;
                    case 1:
                        //a1* item.X2
                        res += resultCheckedCoef[i] * data.X2;
                        break;
                    case 2:
                        //a2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2;
                        break;
                    case 3:
                        //a3* item.X1 +
                        res += resultCheckedCoef[i] * data.X1;
                        break;
                    case 4:
                        //   a4 * item.X1 * item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1;
                        break;
                    case 5:
                        //a5 * item.X1 * item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X2;
                        break;
                    case 6:
                        // a6* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1;
                        break;
                    case 7:
                        //a7* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1;
                        break;
                    case 8:
                        //a8* item.X1* item.X1* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2;
                        break;
                }
            }
            return Math.Abs(data.Y - res);
        }
        private double CalcResDelta(DataTwoFact data, double[] resultCheckedCoef)
        {
            double res = 0;
            for (int i = 0; i < resultCheckedCoef.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        //a0 +
                        res += resultCheckedCoef[i];
                        break;
                    case 1:
                        //a1* item.X2 +
                        res += resultCheckedCoef[i] * data.X2;
                        break;
                    case 2:
                        //a2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2;
                        break;
                    case 3:
                        //a3* item.X1 +
                        res += resultCheckedCoef[i] * data.X1;
                        break;
                    case 4:
                        //   a4 * item.X1 * item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1;
                        break;
                    case 5:
                        //a5 * item.X1 * item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X2;
                        break;
                    case 6:
                        // a6* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1;
                        break;
                    case 7:
                        //a7* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1;
                        break;
                    case 8:
                        //a8* item.X1* item.X1* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2;
                        break;
                    case 9:
                        //a9* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1;
                        break;
                    case 10:
                        //a10* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 11:
                        //a11* item.X2* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 12:
                        //a12* item.X2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2;
                        break;
                    case 13:
                        //a13* item.X2* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1;
                        break;
                    case 14:
                        //a14* item.X2* item.X2* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1;
                        break;
                    case 15:
                        //a15* item.X2* item.X2* item.X2* item.X1* item.X1* item.X1);
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                }
            }
            return Math.Abs(data.Y - res);
        }
        private bool CanCalcFromExelCommandExecute(object p) => (int)DegreeSelected != 0;


    #endregion

    }
}
