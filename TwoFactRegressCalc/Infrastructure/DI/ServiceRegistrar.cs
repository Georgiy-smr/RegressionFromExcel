﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Enums.PolynomialDegrees;
using TwoFactRegressCalc.Infrastructure.DI.Services.Creator;
using TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;
using TwoFactRegressCalc.Infrastructure.DI.Services.JsonFileService;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.RegressionServices;
using TwoFactRegressCalc.Infrastructure.DI.Services.Writer;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc.Infrastructure.DI
{
    internal static class ServiceRegistrar
    {
        internal static IServiceCollection MainWindowAndViewModel(this ServiceCollection services)
            => services
                .AddSingleton<MainViewModel>()
                .AddSingleton(provider =>
                {
                    var vm = provider.GetRequiredService<MainViewModel>();
                    return new MainWindow() { DataContext = vm };
                });

        internal static IServiceCollection ExcelReader(this ServiceCollection services)
            => services
                .AddSingleton<IReadData<DataTwoFact, Degree>, ExcelFileDataReader>();

        internal static IServiceCollection FileDialog(this ServiceCollection service) =>
            service.AddTransient<IDialogService, FileDialogService>();

        internal static IServiceCollection Regression(this ServiceCollection service) =>
            service.AddTransient<IRegression<DataTwoFact>, TwoFactRegressionService>();

        internal static IServiceCollection FilledExcelDoc(this ServiceCollection service) =>
            service.AddTransient<IWriteData<IEnumerable<double[]>>, ExcelFillPressureAndTempData>();

        internal static IServiceCollection FileCreator(this ServiceCollection service) =>
            service.AddTransient<ICreate<Coefficients>, CreateFileWithCoefficients>();

        internal static IServiceCollection JsonFileService(this ServiceCollection service) =>
            service.AddTransient<IJsonFileService<Config>, SettingsJsonFileService>();

    }
     
}

