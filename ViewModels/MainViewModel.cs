using FullControl.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FullControl.ViewModels
{
    public class MainViewModel
    {
        public BotaoDefaultViewModel BotaoDefault { get; set; }
        public ICommand MostrarConsoleCommand { get; }

        public MainViewModel()
        {
            BotaoDefault = new BotaoDefaultViewModel();

            MostrarConsoleCommand = new TestCommand(p => mostrarConsole(), c => true);
        }

        public void mostrarConsole()
        {
            System.Diagnostics.Debug.WriteLine("DEBUG INFO: Botão pressionado via ICommand!!!");
        }
    }


}
