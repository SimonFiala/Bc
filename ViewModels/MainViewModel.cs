using System;
using System.Collections.Generic;
using System.Text;
using Bc.Modules;


namespace Bc.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; }

        public MainViewModel(_8085sim simulator)
        {
            CurrentViewModel = new _8085ViewModel(simulator);
        }
    }
}
