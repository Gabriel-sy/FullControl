using FullControl.Commands;
using FullControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;


namespace FullControl.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
        public event Action<string, object?>? NavegacaoSolicitada;
        public BotaoDefaultViewModel BotaoDefault { get; set; }
        public ICommand NavegarParaOutraPaginaCommand { get; }

        public MainViewModel()
        {
            BotaoDefault = new BotaoDefaultViewModel();

            NavegarParaOutraPaginaCommand = new TestCommand(p => ExecutarNavegacaoOutraPagina(), c => true);
        }

        public void ExecutarNavegacaoOutraPagina()
        {
            object? viewModelParaNovaPagina = this;
            NavegacaoSolicitada?.Invoke("outraPagina.json", viewModelParaNovaPagina);
            System.Diagnostics.Debug.WriteLine("DEBUG INFO: Botão pressionado via ICommand!!!");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
