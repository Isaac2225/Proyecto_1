using DAO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Proyecto_1
{
    /// <summary>
    /// Lógica de interacción para VentanaEditorEscojer.xaml
    /// </summary>
    public partial class VentanaEditorEscojer : Window
    {
        private readonly MyDBContext _context;
        public VentanaEditorEscojer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VentanaEditorParada ventanaParada = new VentanaEditorParada();
            ventanaParada.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
