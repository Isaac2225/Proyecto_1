using DAO;
using Modelo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para VentanaEditorParada.xaml
    /// </summary>
    public partial class VentanaEditorParada : Window
    {
        private readonly MyDBContext _context;
        public VentanaEditorParada()
        {
            InitializeComponent();
            OmplirParades();
        }

        private void OmplirParades()
        {
            DAOParada paradaDAO = new DAOParada(_context);

            dtgParadas.ItemsSource = paradaDAO.GetParadas();
        }
    }
}
