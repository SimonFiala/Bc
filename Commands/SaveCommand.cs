using Bc.Modules;
using Bc.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Bc.Commands
{
    public class SaveCommand : CommandBase
    {
        private readonly _8085ViewModel _8085ViewModel;
        private readonly _8085sim _simulator;

        public SaveCommand(_8085ViewModel _8085ViewModel, _8085sim simulator)
        {
            this._8085ViewModel = _8085ViewModel;
            this._simulator = simulator;
        }

        public override void Execute(object parameter)
        {
            if (_simulator.Path != null)
            {
                try
                {
                    File.WriteAllText(_simulator.Path, _8085ViewModel.CodeTextBox);
                }
                catch
                {
                    string messageBoxText = "Save Failed";
                    string caption = "Error";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;

                    MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                }             
            } 
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, _8085ViewModel.CodeTextBox);
                    _simulator.Path = saveFileDialog.FileName;
                }
            }


        }
    }
}
