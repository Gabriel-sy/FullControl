using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FullControl.Core.Models;

namespace FullControl.Wpf.Core
{
    public class UIBuilder
    {
        private readonly Dictionary<string, Type> _componentRegistry;

        public UIBuilder()
        {
            _componentRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "Label", typeof(Label) },
                { "TextBox", typeof(TextBox) },
                { "Button", typeof(Button) },
                { "StackPanel", typeof(StackPanel) },
                { "Grid", typeof(Grid) },
                { "Border", typeof(Border) },
                { "TextBlock", typeof(TextBlock) },
                { "CheckBox", typeof(CheckBox) },
                { "ComboBox", typeof(ComboBox) },
                { "Canvas", typeof(Canvas) },
                { "BotaoDefault", typeof(FullControl.Controls.BotaoDefault) },
            };
        }

        public FrameworkElement? BuildElement(ElementoUIDefinicao definicao, object? viewModel)
        {
            if (string.IsNullOrEmpty(definicao.Tipo))
            {
                System.Diagnostics.Debug.WriteLine("Erro: Tipo de elemento não definido no JSON.");
                return null;
            }

            if (!_componentRegistry.TryGetValue(definicao.Tipo, out Type? elementType) || elementType == null)
            {
                System.Diagnostics.Debug.WriteLine($"Erro: Tipo de elemento '{definicao.Tipo}' não registrado ou inválido.");
                return new TextBlock { Text = $"Tipo '{definicao.Tipo}' desconhecido." };
            }

            FrameworkElement? element = null;
            try
            {
                element = Activator.CreateInstance(elementType) as FrameworkElement;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao instanciar '{definicao.Tipo}': {ex.Message}");
                return new TextBlock { Text = $"Erro ao criar {definicao.Tipo}" };
            }

            if (element == null) return new TextBlock { Text = $"Falha ao criar {definicao.Tipo}" };

            ApplyProperties(element, definicao, viewModel);

            if (definicao.Filhos != null && definicao.Filhos.Any())
            {
                if (element is Panel panel)
                {
                    foreach (var filhoDef in definicao.Filhos)
                    {
                        var childElement = BuildElement(filhoDef, viewModel);
                        if (childElement != null)
                        {
                            panel.Children.Add(childElement);
                        }
                    }
                }
                else if (element is ContentControl contentCtrl)
                {
                    if (definicao.Filhos.Count == 1)
                    {
                        var childElement = BuildElement(definicao.Filhos.First(), viewModel);
                        if (childElement != null) contentCtrl.Content = childElement;
                    }
                    else if (definicao.Filhos.Count > 1)
                    {
                        System.Diagnostics.Debug.WriteLine($"Aviso: {definicao.Tipo} é ContentControl mas tem múltiplos filhos. Usando um StackPanel para agrupá-los.");
                        var tempPanel = new StackPanel();
                        foreach (var filhoDef in definicao.Filhos)
                        {
                            var childElement = BuildElement(filhoDef, viewModel);
                            if (childElement != null) tempPanel.Children.Add(childElement);
                        }
                        contentCtrl.Content = tempPanel;
                    }
                }
            }
            return element;
        }

        private void ApplyProperties(FrameworkElement element, ElementoUIDefinicao def, object? viewModel)
        {
            // Conteúdo
            if (!string.IsNullOrEmpty(def.Conteudo))
            {
                if (element is ContentControl cc) cc.Content = def.Conteudo;
                else if (element is TextBlock tb) tb.Text = def.Conteudo;
            }

            // Dimensões
            if (def.Largura.HasValue) element.Width = def.Largura.Value;
            if (def.Altura.HasValue) element.Height = def.Altura.Value;

            // Margem
            if (!string.IsNullOrEmpty(def.Margem))
            {
                try { element.Margin = (Thickness)new ThicknessConverter().ConvertFromString(def.Margem); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro ao converter Margem '{def.Margem}': {ex.Message}"); }
            }

            // Alinhamentos (geralmente para quando o elemento está dentro de um container que os respeita)
            if (!string.IsNullOrEmpty(def.AlinhamentoHorizontal) && Enum.TryParse<HorizontalAlignment>(def.AlinhamentoHorizontal, true, out var ha))
                element.HorizontalAlignment = ha;
            if (!string.IsNullOrEmpty(def.AlinhamentoVertical) && Enum.TryParse<VerticalAlignment>(def.AlinhamentoVertical, true, out var va))
                element.VerticalAlignment = va;

            // Propriedades de Canvas (só terão efeito se 'element' for filho direto de um Canvas)
            if (!string.IsNullOrEmpty(def.PosicaoTop) && double.TryParse(def.PosicaoTop, NumberStyles.Any, CultureInfo.InvariantCulture, out var top))
                Canvas.SetTop(element, top);
            if (!string.IsNullOrEmpty(def.PosicaoLeft) && double.TryParse(def.PosicaoLeft, NumberStyles.Any, CultureInfo.InvariantCulture, out var left))
                Canvas.SetLeft(element, left);
            if (!string.IsNullOrEmpty(def.PosicaoRight) && double.TryParse(def.PosicaoRight, NumberStyles.Any, CultureInfo.InvariantCulture, out var right))
                Canvas.SetRight(element, right);
            if (!string.IsNullOrEmpty(def.PosicaoBottom) && double.TryParse(def.PosicaoBottom, NumberStyles.Any, CultureInfo.InvariantCulture, out var bottom))
                Canvas.SetBottom(element, bottom);


            // Propriedades específicas de Layout Containers
            if (element is StackPanel sp && !string.IsNullOrEmpty(def.OrientacaoStackPanel) && Enum.TryParse<Orientation>(def.OrientacaoStackPanel, true, out var orientation))
                sp.Orientation = orientation;

            // Data Binding
            if (!string.IsNullOrEmpty(def.BindingPath) && viewModel != null)
            {
                Binding binding = new Binding(def.BindingPath) { Source = viewModel };
                DependencyProperty? dpToBind = null;
                if (element is TextBox) dpToBind = TextBox.TextProperty;
                else if (element is TextBlock) dpToBind = TextBlock.TextProperty;
                else if (element is CheckBox) dpToBind = CheckBox.IsCheckedProperty;
                else if (element is ComboBox) dpToBind = ComboBox.SelectedValueProperty;
                else if (element is ContentControl && string.IsNullOrEmpty(def.Conteudo)) dpToBind = ContentControl.ContentProperty;
                else if (element is Button) dpToBind = Button.ContentProperty;
                else if (element is Label) dpToBind = Label.ContentProperty;
                else if (element is Border) dpToBind = Border.BackgroundProperty;
                else if (element is Canvas) dpToBind = Canvas.BackgroundProperty;
                else if (element is Panel) dpToBind = Panel.BackgroundProperty; 
                // Adicionar mais mapeamentos...
                if (dpToBind != null) element.SetBinding(dpToBind, binding);
                else System.Diagnostics.Debug.WriteLine($"Aviso: BindingPath '{def.BindingPath}' para '{def.Tipo}' mas propriedade alvo não mapeada.");
            }

            // Ações (Commands)
            if (element is System.Windows.Controls.Primitives.ButtonBase btnBase && !string.IsNullOrEmpty(def.AcaoClick) && viewModel != null)
            {
                Binding commandBinding = new Binding(def.AcaoClick) { Source = viewModel };
                btnBase.SetBinding(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, commandBinding);
            }

            // Processar PropriedadesAdicionais
            if (def.PropriedadesAdicionais != null)
            {
                foreach (var prop in def.PropriedadesAdicionais)
                {
                    try
                    {
                        // Exemplo simples para Background (que é comum para Canvas e outros Panels)
                        if (prop.Key.Equals("Background", StringComparison.OrdinalIgnoreCase) && element is Panel panelElement) // Canvas herda de Panel
                        {
                            panelElement.Background = (Brush)new BrushConverter().ConvertFromString(prop.Value);
                        }
                        else if (prop.Key.Equals("Background", StringComparison.OrdinalIgnoreCase) && element is Control controlElement) // Button, Label herdam de Control
                        {
                            controlElement.Background = (Brush)new BrushConverter().ConvertFromString(prop.Value);
                        }
                        // Adicionar mais mapeamentos para outras propriedades comuns ou usar reflexão (mais complexo)
                        // else {
                        //    var propertyInfo = element.GetType().GetProperty(prop.Key);
                        //    if (propertyInfo != null && propertyInfo.CanWrite) {
                        //        object convertedValue = Convert.ChangeType(prop.Value, propertyInfo.PropertyType);
                        //        propertyInfo.SetValue(element, convertedValue);
                        //    }
                        // }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao aplicar PropriedadeAdicional '{prop.Key}': {ex.Message}");
                    }
                }
            }
        }
    }
}