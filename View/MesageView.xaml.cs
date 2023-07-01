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
using System.Windows.Shapes;

namespace SberPank.View
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class MesageView : Window
    {
        public MesageView(string[] message)
        {
            InitializeComponent();

            fileNameLabel.Content = message[0];
            Title = message[0];

            MessageContent.Text = message[1];
        }

        private void closeBut_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void hideBut_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void fileNameLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
