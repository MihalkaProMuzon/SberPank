using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SberPank.View
{
    class ViewHelp
    {
        public static void setTBNumericOnly(TextBox tb)
        {
            tb.TextChanged += (sender, e) => { 
                if(tb.Text.Length == 0)
                    tb.Text = "0";
            };

            tb.PreviewTextInput += (sender, e) =>
            {
                if (!Char.IsDigit(e.Text, 0))
                    e.Handled = true;
                
            };


        }

    }
}
