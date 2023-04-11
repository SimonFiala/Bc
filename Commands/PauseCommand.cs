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
    public class PauseCommand : CommandBase
    {
        private readonly _8085ViewModel _8085ViewModel;
        private readonly _8085sim _simulator;

        public PauseCommand(_8085ViewModel _8085ViewModel, _8085sim simulator)
        {
            this._8085ViewModel = _8085ViewModel;
            _simulator = simulator;
        }

        public override void Execute(object parameter)
        {
            _simulator.IsPaused = true;
            _8085ViewModel.RegisterValues = _simulator.Registers.Values.ToList().ConvertAll<string>(x => x.ToString("X2"));
            _8085ViewModel.SpecialRegisterValues = _simulator.SpecialRegisters.Values.ToList().ConvertAll<string>(x => x.ToString("X2"));
            _8085ViewModel.FlagsValues = _simulator.Flags.Values.ToList().Select(x => x ? 1 : 0).ToList();
        }
    }

}
