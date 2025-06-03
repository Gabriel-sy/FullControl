using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullControl.Core.Models
{
    public class ElementoUIDefinicao
    {
        public string Tipo { get; set; } = string.Empty;
        public string? Conteudo { get; set; }
        public double? Largura { get; set; }
        public double? Altura { get; set; }
        public string? Margem { get; set; }
        public string? AlinhamentoHorizontal { get; set; }
        public string? AlinhamentoVertical { get; set; }

        public string? PosicaoTop { get; set; }
        public string? PosicaoLeft { get; set; }
        public string? PosicaoRight { get; set; }
        public string? PosicaoBottom { get; set; }
        public string? OrientacaoStackPanel { get; set; }
        public List<DefinicaoBinding>? Bindings { get; set; }
        public string? AcaoClick { get; set; }
        public List<ElementoUIDefinicao>? Filhos { get; set; }
        public Dictionary<string, string>? PropriedadesAdicionais { get; set; }
    }
}
