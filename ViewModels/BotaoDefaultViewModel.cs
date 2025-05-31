using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullControl.ViewModels
{
    public class BotaoDefaultViewModel
    {
        public string texto { get; set; }
        public string? corFundo { get; set; }
        public string input { get; set; } = string.Empty;

        public BotaoDefaultViewModel()
        {
            texto = "Xesquedele";
            corFundo = "Black";
        }
    }
}
