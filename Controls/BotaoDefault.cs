    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    namespace FullControl.Controls
    {
        public class BotaoDefault : Button
        {
            private string? _conteudoOriginal;
            private bool _foiClicado = false;
            public string texto = "Data binding";

            public BotaoDefault()
            {
                this.Background = Brushes.DodgerBlue;
                this.Foreground = Brushes.White;
                this.Padding = new Thickness(12, 6, 12, 6);
                this.BorderThickness = new Thickness(0);
                this.Cursor = System.Windows.Input.Cursors.Hand;

                this.Loaded += BotaoDefault_Loaded;

                this.Click += BotaoDefault_Click;
            }

            private void BotaoDefault_Loaded(object sender, RoutedEventArgs e)
            {
                if (!_foiClicado)
                {
                    _conteudoOriginal = this.Content?.ToString();
                }
                if (string.IsNullOrEmpty(_conteudoOriginal))
                {
                    _conteudoOriginal = "Botão"; 
                    if (!_foiClicado)
                    {
                        this.Content = _conteudoOriginal;
                    }
                }
            }

            private void BotaoDefault_Click(object sender, RoutedEventArgs e)
            {
                if (_foiClicado)
                {
                    this.Content = _conteudoOriginal;
                    this.Background = Brushes.DodgerBlue;
                    _foiClicado = false;
                }
                else
                {
                    if (this.Content?.ToString() != "Clicado!")
                    {
                        _conteudoOriginal = this.Content?.ToString();
                    }
                    this.Content = "Clicado!";
                    this.Background = Brushes.RoyalBlue; 
                    _foiClicado = true;
                }
            }
        }
    }