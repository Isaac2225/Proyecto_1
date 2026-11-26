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
    /// Lógica de interacción para Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {

        private readonly MyDBContext _context;
        public Window4()
        {
            InitializeComponent();
            _context = new MyDBContext();
            CargaDataGrids();
        }

        private void CargaDataGrids()
        {
            DAOUsuario usuarioDAO = new DAOUsuario(_context);
            DAOTrayecto trayectoDAO = new DAOTrayecto(_context);

            dtgTrayectos.ItemsSource = trayectoDAO.GetTrayectos();
            dtgUsuarios.ItemsSource = usuarioDAO.GetUsuarios();
        }
    }
}
