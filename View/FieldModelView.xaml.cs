using SberPank.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SberPank
{
    public partial class FieldModelView : UserControl
    {
        public event Action FieldRemoved;

        Field fld;

        public FieldModelView(Field fld)
        {
            this.fld = fld;
            InitializeComponent();

            ViewHelp.setTBNumericOnly(offsetTB);
            ViewHelp.setTBNumericOnly(indexTB);

            nameTB.Text = fld.name;
            nameTB.TextChanged += (sender, e) => { fld.name = nameTB.Text; };

            fromTB.Text = fld.From;
            fromTB.TextChanged += (sender, e) => { fld.From = fromTB.Text; };

            toTB.Text = fld.To;
            toTB.TextChanged += (sender, e) => { fld.To = toTB.Text; };

            indexTB.Text = fld.index.ToString();
            indexTB.TextChanged += (sender, e) => { fld.index = int.Parse( indexTB.Text ); };
            

            offsetTB.Text = fld.offset.ToString();
            offsetTB.TextChanged += (sender, e) => { fld.offset = int.Parse(offsetTB.Text); };
            

            repeatsCB.IsChecked = fld.checkRepeats;
            repeatsCB.Checked += (sender, e) => { fld.checkRepeats = true; };
            repeatsCB.Unchecked += (sender, e) => { fld.checkRepeats = false; };

            optionalCB.IsChecked = fld.optional;
            optionalCB.Checked += (sender, e) => { fld.optional = true; };
            optionalCB.Unchecked += (sender, e) => { fld.optional = false; };


            cellcharsTB.Text = fld.cellChars;
            cellcharsTB.TextChanged += (sender, e) => { fld.cellChars = cellcharsTB.Text; };

        }

        private void delFieldBut_Click(object sender, RoutedEventArgs e)
        {
            FieldRemoved.Invoke();
        }
    }
}
