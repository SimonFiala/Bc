using Bc.Exceptions;
using Bc.Modules;
using Bc.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Bc.Commands
{
    internal class AssembleCommand : CommandBase
    {
        private readonly _8085ViewModel _8085ViewModel;
        private readonly _8085sim _simulator;

        public AssembleCommand(_8085ViewModel _8085ViewModel, _8085sim simulator)
        {
            this._8085ViewModel = _8085ViewModel;
            _simulator = simulator;
        }

        public override void Execute(object parameter)
        {
            if (_simulator.Path == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, _8085ViewModel.CodeTextBox);
                    _simulator.Path = saveFileDialog.FileName;
                } else
                {
                    return;
                }
            }
            try
            {
                _simulator.ReadLines();
                _8085ViewModel.MachineCodeTextBox = _simulator.TranslatedCode.ToString();
                _8085ViewModel.RegisterValues = _simulator.Registers.Values.ToList().ConvertAll<string>(x => x.ToString("X2"));
                _8085ViewModel.SpecialRegisterValues = _simulator.SpecialRegisters.Values.ToList().ConvertAll<string>(x => x.ToString("X2"));
                _8085ViewModel.FlagsValues = _simulator.Flags.Values.ToList().Select(x => x ? 1 : 0).ToList();
            }
            catch (WrongArgumentsException e)
            {
                string messageBoxText = e.Message;
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;

                MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }



        }
    }
}
