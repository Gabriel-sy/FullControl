using System.Collections.Generic;

namespace FullControl.Core.Models
{
    public class DefinicaoBinding
    {
        public string TipoAlvo { get; set; } = string.Empty;

        public string BindingPath { get; set; } = string.Empty;

        public string? Modo { get; set; }
        public List<string>? Validacoes { get; set; }
    }

}