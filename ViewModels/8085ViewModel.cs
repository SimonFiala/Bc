using System;
using System.Collections.Generic;
using System.Text;
using Bc.Modules;
using System.Linq;
using System.Windows.Input;
using Bc.Commands;

namespace Bc.ViewModels
{
    public class _8085ViewModel : ViewModelBase
    {

        private readonly _8085sim _8085Sim;

        private List<string> _registerNames;


        private bool _isDebugMode = false;

        public bool IsDebugMode { get { return _isDebugMode; } set { _isDebugMode = value; OnPropertyChanged(nameof(IsDebugMode)); } }

        public bool IsNotDebugMode { get { return !_isDebugMode; } }

        public List<string> RegisterNames
        {
            get
            {
                return _registerNames;
            }
            set
            {
                _registerNames = value;
                OnPropertyChanged(nameof(RegisterNames));
            }
        }

        private List<string> _registerValues;
        public List<string> RegisterValues
        {
            get
            {
                return _registerValues;
            }
            set
            {
                _registerValues = value;
                OnPropertyChanged(nameof(RegisterValues));
            }
        }

        public List<string> SpecialRegisterNames
        {
            get
            {
                return _8085Sim.SpecialRegisters.Keys.ToList();
            }
        }

        private List<string> _specialRegisterValues;
        public List<string> SpecialRegisterValues
        {
            get
            {
                return _specialRegisterValues;
            }
            set
            {
                _specialRegisterValues = value;
                OnPropertyChanged(nameof(SpecialRegisterValues));
            }
        }

        public List<string> FlagsNames
        {
            get
            {
                return _8085Sim.Flags.Keys.ToList();
            }
        }

        private List<int> _flagsValues;
        public List<int> FlagsValues
        {
            get
            {
                return _flagsValues;
            }
            set
            {
                _flagsValues = value;
                OnPropertyChanged(nameof(FlagsValues));
            }
        }

        private string _codeTextBox;
        public string CodeTextBox
        {
            get
            {
                return _codeTextBox;
            }
            set
            {
                _codeTextBox = value;
                OnPropertyChanged(nameof(CodeTextBox));
            }
        }

        private string _machineCodeTextBox;
        public string MachineCodeTextBox
        {
            get
            {
                return _machineCodeTextBox;
            }
            set
            {
                _machineCodeTextBox = value;
                OnPropertyChanged(nameof(MachineCodeTextBox));
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ContinueCommand { get; }
        public ICommand StepCommand { get; }
        public ICommand AssembleCommand { get; }

        public ICommand StopCommand { get; }


        public _8085ViewModel(_8085sim simulator)
        {
            _8085Sim = simulator;
            _registerNames = simulator.Registers.Keys.ToList();
            _registerValues = simulator.Registers.Values.ToList().ConvertAll<string>(x => x.ToString("X2"));
            _specialRegisterValues = simulator.SpecialRegisters.Values.ToList().ConvertAll<string>(x => x.ToString("X2"));
            _flagsValues = simulator.Flags.Values.ToList().Select(x => x ? 1 : 0).ToList();


            LoadCommand = new LoadCommand(this, simulator);
            SaveCommand = new SaveCommand(this, simulator);
            RunCommand = new RunCommand(this, simulator);
            PauseCommand = new PauseCommand(this, simulator);
            ContinueCommand = new ContinueCommand(this, simulator);
            StepCommand = new StepCommand(this, simulator);
            AssembleCommand = new AssembleCommand(this, simulator);
            StopCommand = new StopCommand(this, simulator);
        }
    }
}
