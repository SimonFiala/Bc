using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bc.Views;
using Bc.ViewModels;
using Bc.Modules;
using System.Linq;
using Bc.Exceptions;
using System.Windows;
namespace Bc.Commands
{
    public class StopCommand : CommandBase
    {
        private readonly _8085ViewModel _8085ViewModel;
        private readonly _8085sim _simulator;

        public StopCommand(_8085ViewModel _8085ViewModel, _8085sim simulator)
        {
            this._8085ViewModel = _8085ViewModel;
            _simulator = simulator;
        }

        public override void Execute(object parameter)
        {
            _simulator.IsPaused = false;
            _simulator.IsHalted = true;
            _8085ViewModel.IsDebugMode = false;
        }

    }
}
