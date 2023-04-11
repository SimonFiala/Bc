using Bc.Modules;
using Bc.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly _8085sim _simulator;
        public App()
        {
            _simulator = new _8085sim();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.Properties["Simulator"] = new _8085sim();
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_simulator)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }

        private void CutContractKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                MessageBox.Show("CTRL + S");
            }
        }

    }
}
