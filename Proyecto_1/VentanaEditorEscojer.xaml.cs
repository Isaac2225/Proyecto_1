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
                txtOrigen.Text = selectedTrayecto.Origen?.Nombre ?? "";
                txtDestino.Text = selectedTrayecto.Destino?.Nombre ?? "";
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
            using (var transaccio = _context.Database.BeginTransaction())
            {
                try
                {
                    DAOEstacion estacionDAO = new DAOEstacion(_context);
                    DAOTrayecto trayectoDAO = new DAOTrayecto(_context);

                    // 1. Buscar las estaciones por nombre usando tu DAOEstacion
                    Estacion origen = estacionDAO.GetEstacion(txtOrigen.Text);
                    Estacion destino = estacionDAO.GetEstacion(txtDestino.Text);

                    if (origen == null || destino == null)
                    {
                        MessageBox.Show("La estación de origen o destino no existe en la base de datos.");
                        transaccio.Rollback();
                        return;
                    }

                    // 2. Comprobar que hay un medio de transporte seleccionado
                    if (cboMedioTransporte.SelectedItem is not MedioTransporte medio)
                    {
                        MessageBox.Show("Selecciona un medio de transporte.");
                        transaccio.Rollback();
                        return;
                    }

                    // 3. Crear el trayecto con Origen y Destino asignados
                    Trayecto nuevoTrayecto = new Trayecto(txtNombre.Text, DateTime.Now, medio);
                    nuevoTrayecto.Origen = origen;
                    nuevoTrayecto.Destino = destino;

                    // 4. Insertar y confirmar
                    if (trayectoDAO.InsertTrayecto(nuevoTrayecto))
                    {
                        transaccio.Commit();
                        MessageBox.Show("Trayecto insertado correctamente.");
                        // Aquí puedes volver a cargar dtgTrayectos para ver el nuevo registro
                    }
                }
                catch (Exception ex)
                {
                    transaccio.Rollback();
                    MessageBox.Show("Error al insertar: " + ex.Message);
                }
            }
        }

        private void Button_Click_Modificar(object sender, RoutedEventArgs e)
        {

        }

        private void dtgParadas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
