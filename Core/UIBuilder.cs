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
        private readonly Dictionary<string, Type> _validatorRegistry;

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
                { "InputDefault", typeof(FullControl.Controls.InputDefault) }
            };

            _validatorRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "email", typeof(FullControl.Validators.EmailValidator) }
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

            if (!string.IsNullOrEmpty(def.Conteudo))
            {
                if (element is ContentControl cc && cc.Content == null)
                {
                    bool contentIsBound = def.Bindings?.Any(b =>
                        b.TipoAlvo.Equals("texto", StringComparison.OrdinalIgnoreCase) ||
                        b.TipoAlvo.Equals("content", StringComparison.OrdinalIgnoreCase)) ?? false;
                    // Só aplica se não tiver binding.
                    if (!contentIsBound)
                    {
                        cc.Content = def.Conteudo;
                    }
                }
                else if (element is TextBlock tb)
                {
                    bool textIsBound = def.Bindings?.Any(b =>
                        b.TipoAlvo.Equals("texto", StringComparison.OrdinalIgnoreCase)) ?? false;
                    if (!textIsBound)
                    {
                        tb.IsHitTestVisible = true;
                        tb.Text = def.Conteudo;
                    }
                }
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

            // Alinhamentos
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

            if (def.Bindings != null && def.Bindings.Any() && viewModel != null)
            {
                foreach (var bindingDef in def.Bindings)
                {
                    if (string.IsNullOrEmpty(bindingDef.TipoAlvo) || string.IsNullOrEmpty(bindingDef.BindingPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"Aviso: Binding incompleto no JSON. TipoAlvo='{bindingDef.TipoAlvo}', Path='{bindingDef.BindingPath}'.");
                        continue;
                    }

                    Binding binding = new Binding(bindingDef.BindingPath) { Source = viewModel };

                    // Definir Modo do Binding (opcional)
                    if (!string.IsNullOrEmpty(bindingDef.Modo) && Enum.TryParse<BindingMode>(bindingDef.Modo, true, out var mode))
                    {
                        binding.Mode = mode;
                    }

                    bool hasValidationRules = false;
                    if (bindingDef.Validacoes != null && bindingDef.Validacoes.Any())
                    {
                        hasValidationRules = true;
                        foreach (string nomeValidador in bindingDef.Validacoes)
                        {
                            if (_validatorRegistry.TryGetValue(nomeValidador, out Type? tipoValidador) && tipoValidador != null)
                            {
                                try
                                {
                                    ValidationRule? regra = Activator.CreateInstance(tipoValidador) as ValidationRule;
                                    if (regra != null)
                                    {
                                        binding.ValidationRules.Add(regra);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Erro ao instanciar validador '{nomeValidador}': {ex.Message}");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Aviso: Validador '{nomeValidador}' não registrado no UIBuilder.");
                            }
                        }
                    }

                    DependencyProperty? dpToBind = ResolveDependencyProperty(element, bindingDef.TipoAlvo);

                    if (dpToBind != null)
                    {
                        binding.NotifyOnValidationError = true;
                        element.SetBinding(dpToBind, binding);

                        if (hasValidationRules)
                        {
                            
                            ConfigureFeedbackDeErroVisual(element);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Aviso: Não foi possível resolver a DependencyProperty para TipoAlvo '{bindingDef.TipoAlvo}' no elemento '{element.GetType().Name}'. BindingPath='{bindingDef.BindingPath}'.");
                    }
                }
            }

            // OnClick
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
                        if (prop.Key.Equals("Background", StringComparison.OrdinalIgnoreCase) && element is Panel panelElement) 
                        {
                            panelElement.Background = (Brush)new BrushConverter().ConvertFromString(prop.Value);
                        }
                        else if (prop.Key.Equals("Background", StringComparison.OrdinalIgnoreCase) && element is Control controlElement)
                        {
                            controlElement.Background = (Brush)new BrushConverter().ConvertFromString(prop.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao aplicar PropriedadeAdicional '{prop.Key}': {ex.Message}");
                    }
                }
            }
        }

        private void ConfigureFeedbackDeErroVisual(FrameworkElement element)
        {
            object? originalToolTip = element.ToolTip;
            Brush? originalBorderBrush = null;
            Thickness originalBorderThickness = new Thickness(0);

            if (element is Control controlElement)
            {
                originalBorderBrush = controlElement.BorderBrush;
                originalBorderThickness = controlElement.BorderThickness;
            }
            Validation.AddErrorHandler(element, (sender, args) =>
            {
                if (sender == null)
                {
                    System.Diagnostics.Debug.WriteLine("VALIDATION ERROR HANDLER: 'sender' é NULL!");
                    return;
                }

                var erroredElement = sender as FrameworkElement;

                if (args.Action == ValidationErrorEventAction.Added)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro de validação adicionado a: {erroredElement.Name ?? erroredElement.GetType().Name}, Erro: {args.Error.ErrorContent}");

                    if (erroredElement is Control c)
                    {
                        c.BorderBrush = Brushes.Red;
                        c.BorderThickness = new Thickness(1);
                    }
                    var firstError = Validation.GetErrors(erroredElement).FirstOrDefault();
                    
                    System.Diagnostics.Debug.WriteLine(firstError);
                    if (firstError != null)
                    {
                        erroredElement.ToolTip = firstError.ErrorContent?.ToString();
                    }
                }
                else if (args.Action == ValidationErrorEventAction.Removed)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro de validação removido de: {erroredElement.Name ?? erroredElement.GetType().Name}");

                    if (!Validation.GetHasError(erroredElement))
                    {
                        if (erroredElement is Control c)
                        {
                            c.BorderBrush = originalBorderBrush;
                            c.BorderThickness = originalBorderThickness;
                        }
                        erroredElement.ToolTip = originalToolTip;
                    }
                    else
                    {
                        var firstError = Validation.GetErrors(erroredElement).FirstOrDefault();
                        if (firstError != null)
                        {
                            erroredElement.ToolTip = firstError.ErrorContent?.ToString();
                        }
                        else 
                        {
                            erroredElement.ToolTip = originalToolTip; 
                        }
                    }
                }
            });
        }

        private DependencyProperty? ResolveDependencyProperty(FrameworkElement element, string tipoAlvo)
        {
            string targetPropName = tipoAlvo.ToLowerInvariant();
            switch (targetPropName)
            {
                case "texto":
                case "content":
                    if (element is TextBox) return TextBox.TextProperty;
                    if (element is TextBlock) return TextBlock.TextProperty;
                    if (element is ContentControl) return ContentControl.ContentProperty; 
                    break;
                case "background":
                case "corfundo":
                    if (element is Control) return Control.BackgroundProperty;
                    if (element is Panel) return Panel.BackgroundProperty;
                    if (element is Border) return Border.BackgroundProperty;
                    break;
                case "foreground":
                case "cortexto":
                    if (element is Control) return Control.ForegroundProperty;
                    if (element is TextBlock) return TextBlock.ForegroundProperty;
                    break;
                case "visibilidade":
                case "visibility":
                    return UIElement.VisibilityProperty;
                case "habilitado":
                case "isenabled":
                    return UIElement.IsEnabledProperty;
                case "largura":
                case "width":
                    return FrameworkElement.WidthProperty;
                case "altura":
                case "height":
                    return FrameworkElement.HeightProperty;
                case "itens":
                case "itemssource":
                    if (element is ItemsControl) return ItemsControl.ItemsSourceProperty;
                    break;
                case "valorselecionado":
                case "selectedvalue":
                    if (element is System.Windows.Controls.Primitives.Selector selector) return System.Windows.Controls.Primitives.Selector.SelectedValueProperty;
                    break;
                case "marcado":
                case "ischecked":
                    if (element is System.Windows.Controls.Primitives.ToggleButton toggle) return System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty;
                    break;
            }
            System.Diagnostics.Debug.WriteLine($"Não foi possível encontrar uma DependencyProperty correspondente para TipoAlvo: '{tipoAlvo}' no elemento do tipo '{element.GetType().Name}'.");
            return null;
        }
    }
}