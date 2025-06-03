using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FullControl.ViewModels
{
    public class BotaoDefaultViewModel : INotifyPropertyChanged
    {
        public string texto { get; set; }
        public string? corFundo { get; set; }
        public string input { get; set; } = string.Empty;

        public BotaoDefaultViewModel()
        {
            texto = "aaaaaaaaa";
            corFundo = "Black";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
