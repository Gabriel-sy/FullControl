using FullControl.Core;
using FullControl.Core.Models;
using FullControl.ViewModels;
using FullControl.Wpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FullControl.Views
{
    /// <summary>
    /// Lógica interna para DynamicViewWindows.xaml
    /// </summary>
    public partial class DynamicViewWindow : Window
    {
        private readonly JsonParser _jsonParser;
        private readonly UIBuilder _uiBuilder;
        private object? _viewModel; 

        public DynamicViewWindow(string jsonFileName = "test.json", object? viewModel = null)
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;

            _jsonParser = new JsonParser();
            _uiBuilder = new UIBuilder();

            SetupViewModel(viewModel);
            LoadAndRenderUI(jsonFileName);
        }

        private void SetupViewModel(object? newViewModel)
        {
            if (_viewModel is MainViewModel oldMainVm)
            {
                oldMainVm.NavegacaoSolicitada -= HandleNavigationRequested;
            }

            _viewModel = newViewModel;
            this.DataContext = _viewModel;
            if (_viewModel is MainViewModel newMainVm)
            {
                newMainVm.NavegacaoSolicitada += HandleNavigationRequested;
            }
        }

        private void HandleNavigationRequested(string jsonFileName, object? viewModelForNewPage)
        {
            if (this._viewModel != viewModelForNewPage)
            {
                SetupViewModel(viewModelForNewPage);
            }
            LoadAndRenderUI(jsonFileName);
        }

        public async void LoadAndRenderUI(string jsonFileName)
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonDefinitions", jsonFileName);

            if (!File.Exists(jsonFilePath))
            {
                MainCanvasHost.Children.Clear();
                MainCanvasHost.Children.Add(new TextBlock { Text = $"Arquivo JSON não encontrado: {jsonFileName}", Foreground = Brushes.Red, Margin = new Thickness(10) });
                return;
            }

            TelaDefinicao? telaDef = await _jsonParser.ParseJsonFileAsync(jsonFilePath);

            if (telaDef == null)
            {
                MainCanvasHost.Children.Clear();
                MainCanvasHost.Children.Add(new TextBlock { Text = $"Erro ao parsear o arquivo JSON: {jsonFileName}", Foreground = Brushes.Red, Margin = new Thickness(10) });
                return;
            }

            if (!string.IsNullOrEmpty(telaDef.TituloTela))
            {
                this.Title = telaDef.TituloTela;
            }

            MainCanvasHost.Children.Clear();


            if (telaDef.ComponenteRaiz != null)
            {
                if (telaDef.ComponenteRaiz.PropriedadesAdicionais != null &&
                    telaDef.ComponenteRaiz.PropriedadesAdicionais.TryGetValue("Background", out string? bgValue) &&
                    !string.IsNullOrEmpty(bgValue))
                {
                    try
                    {
                        MainCanvasHost.Background = (Brush)new BrushConverter().ConvertFromString(bgValue);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao aplicar background do ComponenteRaiz ao MainCanvasHost: {ex.Message}");
                    }
                }
                if (telaDef.ComponenteRaiz.Filhos != null)
                {
                    foreach (var filhoDef in telaDef.ComponenteRaiz.Filhos)
                    {
                        FrameworkElement? childElement = _uiBuilder.BuildElement(filhoDef, _viewModel);
                        if (childElement != null)
                        {
                            MainCanvasHost.Children.Add(childElement);
                        }
                    }
                }
            }
            else
            {
                MainCanvasHost.Children.Add(new TextBlock { Text = "JSON não define um ComponenteRaiz ou ComponenteRaiz não tem filhos para exibir.", Margin = new Thickness(10) });
            }
        }

        private void DynamicViewWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel is MainViewModel mainVm)
            {
                mainVm.NavegacaoSolicitada -= HandleNavigationRequested;
            }
        }
    }
}
