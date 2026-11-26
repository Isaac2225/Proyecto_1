using DAO;
using Modelo;
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

        private void Button_Click_Bloquear(object sender, RoutedEventArgs e)
        {
            if (dtgUsuarios.SelectedItem is not Usuario usuarioSeleccionado)
            {
                MessageBox.Show("Selecciona un usuario primero.");
                return;
            }

            if (usuarioSeleccionado.Bloqueado)
            {
                MessageBox.Show("Este usuario ya está bloqueado.");
                return;
            }

            try
            {
                usuarioSeleccionado.Bloqueado = true;
                DAOUsuario daoUsuario = new DAOUsuario(_context);

                using (var transaccio = _context.Database.BeginTransaction())
                {
                    if (daoUsuario.UpdateUsuario(usuarioSeleccionado))
                    {
                        transaccio.Commit();
                        dtgUsuarios.Items.Refresh();
                        MessageBox.Show($"Usuario {usuarioSeleccionado.Nombre} bloqueado correctamente.");
                    }
                    else
                    {
                        usuarioSeleccionado.Bloqueado = false;
                        MessageBox.Show("No se pudo bloquear el usuario.");
                    }
                }
            }
            catch (Exception ex)
            {
                usuarioSeleccionado.Bloqueado = false;
                MessageBox.Show("Error al bloquear: " + ex.Message);
            }
        }
    }
}
