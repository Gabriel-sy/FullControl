using FullControl.Core;
using FullControl.Core.Models;
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
        private readonly object? _viewModel; // Seu ViewModel para a tela

        // Construtor que recebe o nome do arquivo JSON e o ViewModel
        public DynamicViewWindow(string jsonFileName = "test.json", object? viewModel = null)
        {
            InitializeComponent();

            _jsonParser = new JsonParser();
            _uiBuilder = new UIBuilder();
            _viewModel = viewModel;

            // Define o DataContext da Janela para o ViewModel fornecido.
            // Isso permite que bindings dentro da janela (ex: Title) e na UI gerada funcionem.
            if (_viewModel != null)
            {
                this.DataContext = _viewModel;
            }

            LoadAndRenderUI(jsonFileName);
        }

        private async void LoadAndRenderUI(string jsonFileName)
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonDefinitions", jsonFileName);

            if (!File.Exists(jsonFilePath))
            {
                DynamicContentHost.Content = new TextBlock { Text = $"Arquivo JSON não encontrado: {jsonFileName}", Foreground = Brushes.Red };
                return;
            }

            TelaDefinicao? telaDef = await _jsonParser.ParseJsonFileAsync(jsonFilePath);

            if (telaDef == null)
            {
                DynamicContentHost.Content = new TextBlock { Text = $"Erro ao parsear o arquivo JSON: {jsonFileName}", Foreground = Brushes.Red };
                return;
            }

            // Define o título da janela se especificado no JSON e não houver binding no ViewModel
            if (!string.IsNullOrEmpty(telaDef.TituloTela))
            {
                // Se o DataContext (ViewModel) tiver uma propriedade TituloTela e a Window.Title estiver bindada a ela,
                // o ideal é que o ViewModel controle o título.
                // Mas para um override direto do JSON:
                this.Title = telaDef.TituloTela;
            }


            if (telaDef.ComponenteRaiz != null)
            {
                FrameworkElement? rootUiElement = _uiBuilder.BuildElement(telaDef.ComponenteRaiz, _viewModel);
                if (rootUiElement != null)
                {
                    DynamicContentHost.Content = rootUiElement;
                }
                else
                {
                    DynamicContentHost.Content = new TextBlock { Text = "Falha ao construir o elemento raiz da UI.", Foreground = Brushes.Red };
                }
            }
            else
            {
                DynamicContentHost.Content = new TextBlock { Text = "JSON não define um ComponenteRaiz.", Foreground = Brushes.Red };
            }
        }
    }
}
