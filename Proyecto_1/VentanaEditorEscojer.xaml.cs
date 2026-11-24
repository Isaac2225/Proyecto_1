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
    /// Lógica de interacción para VentanaEditorEscojer.xaml
    /// </summary>
    public partial class VentanaEditorEscojer : Window
    {
        private readonly MyDBContext _context;
        public VentanaEditorEscojer()
        {
            InitializeComponent();
            _context = new MyDBContext();
            CargarDataGrids();
            LlenarCboMedios();
        }

        private void CargarDataGrids()
        {
            DAOTrayecto trayectoDAO = new DAOTrayecto(_context);
            dtgTrayectos.ItemsSource = trayectoDAO.GetTrayectos();
            DAOParada paradaDAO = new DAOParada(_context);
            dtgParadas.ItemsSource = paradaDAO.GetParadas();
        }

        private void Button_Click_Eliminar(object sender, RoutedEventArgs e)
        {
            if (dtgTrayectos.SelectedItem is Trayecto selectedTrayecto)
            {
                using (var transaccion = _context.Database.BeginTransaction())
                {
                    try
                    {
                        DAOTrayecto trayectoDAO = new DAOTrayecto(_context);
                        bool eliminado = trayectoDAO.DeleteTrayecto(selectedTrayecto.Id);

                        if (eliminado)
                        {
                            transaccion.Commit();
                            CargarDataGrids();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        MessageBox.Show("Error al eliminar: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un trayecto para eliminar.");
            }
        }

        private void dtgTrayectos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtgTrayectos.SelectedItem is Trayecto selectedTrayecto)
            {
                txtNombre.Text = selectedTrayecto.Nombre;
                txtOrigen.Text = selectedTrayecto.Origen.ToString();
                txtDestino.Text = selectedTrayecto.Destino.ToString();

            }
        }

        private void LlenarCboMedios()
        {
            cboMedioTransporte.Items.Add("Bus");
            cboMedioTransporte.Items.Add("Tren");
            cboMedioTransporte.Items.Add("Avión");
            cboMedioTransporte.Items.Add("Barco");
        }

        private void Button_Click_Insertar(object sender, RoutedEventArgs e)
        {
            DAOTrayecto trayectoDAO = new DAOTrayecto(_context);

            
        }

        private void Button_Click_Modificar(object sender, RoutedEventArgs e)
        {

        }
    }
}
