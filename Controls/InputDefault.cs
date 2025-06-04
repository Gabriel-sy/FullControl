using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FullControl.Controls
{
    public class InputDefault : TextBox
    {
        public InputDefault()
        {
            this.FontSize = 14;
            this.Background = new SolidColorBrush(Colors.LightGray);
            this.BorderThickness = new System.Windows.Thickness(1);
            this.BorderBrush = new SolidColorBrush(Colors.Aqua);
        }
        
    }

}
