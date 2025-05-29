using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullControl.ViewModels
{
    public class MainViewModel
    {
        public BotaoDefaultViewModel BotaoDefault { get; set; }

        public MainViewModel()
        {
            BotaoDefault = new BotaoDefaultViewModel();
        }
    }
}
