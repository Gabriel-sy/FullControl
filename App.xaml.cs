using FullControl.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using FullControl.ViewModels;

namespace FullControl
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string arquivoJsonInicial = "test.json";
            string arquivoTemaInicial = "DefaultTheme.json";
                                                                 
            object? viewModelInicial = new MainViewModel(); 

            DynamicViewWindow mainWindow = new DynamicViewWindow(arquivoJsonInicial, viewModelInicial, arquivoTemaInicial);
            mainWindow.Show();
        }
    }

}
