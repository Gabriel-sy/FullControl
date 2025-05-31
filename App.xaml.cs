using FullControl.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using FullControl.ViewModels;

namespace FullControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Exemplo: Carregar uma tela JSON padrão ao iniciar
            string arquivoJsonInicial = "test.json"; // Ou o nome do seu JSON de tela inicial
                                                                  // object? viewModelInicial = new MinhaTelaViewModel(); // Crie uma instância do seu ViewModel se necessário
            object? viewModelInicial = new MainViewModel(); // Ou passe null se não tiver um ViewModel para este momento

            DynamicViewWindow mainWindow = new DynamicViewWindow(arquivoJsonInicial, viewModelInicial);
            mainWindow.Show();
        }
    }

}
