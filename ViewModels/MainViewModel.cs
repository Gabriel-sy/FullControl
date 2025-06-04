using FullControl.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace FullControl.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event Action<string, object?, string?>? NavegacaoSolicitada;
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
            NavegacaoSolicitada?.Invoke("outraPagina.json", viewModelParaNovaPagina, "RedTheme.json");
            System.Diagnostics.Debug.WriteLine("DEBUG INFO: Botão pressionado via ICommand!!!");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
