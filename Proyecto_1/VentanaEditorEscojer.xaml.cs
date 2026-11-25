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
        private bool camposLlenos = false;
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
            camposLlenos = true;
        }

        private void LlenarCboMedios()  
        {
            cboMedioTransporte.Items.Add("Autobús");
            cboMedioTransporte.Items.Add("Tren");
            cboMedioTransporte.Items.Add("Metro");
            cboMedioTransporte.Items.Add("Tramvia");
            cboMedioTransporte.Items.Add("Bicicleta");
            cboMedioTransporte.Items.Add("Coche compartido");
            cboMedioTransporte.Items.Add("Furgoneta");
            cboMedioTransporte.Items.Add("Taxi");
            cboMedioTransporte.Items.Add("Barco");
            cboMedioTransporte.Items.Add("Patinete Eléctrico");
        }

        private void Button_Click_Insertar(object sender, RoutedEventArgs e)
        {
            using (var transaccio = _context.Database.BeginTransaction())
            {
                try
                {
                    DAOEstacion estacionDAO = new DAOEstacion(_context);
                    DAOTrayecto trayectoDAO = new DAOTrayecto(_context);

                    Estacion origen = estacionDAO.GetEstacion(txtOrigen.Text);
                    Estacion destino = estacionDAO.GetEstacion(txtDestino.Text);

                    if (origen == null || destino == null)
                    {
                        MessageBox.Show("La estación de origen o destino no existe en la base de datos.");
                        transaccio.Rollback();
                        return;
                    }

                    if (cboMedioTransporte.SelectedIndex == -1)
                    {
                        MessageBox.Show("Selecciona un medio de transporte.");
                        transaccio.Rollback();
                        return;
                    }

                    MedioTransporte medio = new MedioTransporte(cboMedioTransporte.SelectedItem.ToString());
                    medio.Id = cboMedioTransporte.SelectedIndex + 1;

                    Trayecto nuevoTrayecto = new Trayecto(txtNombre.Text, DateTime.Now, medio);
                    nuevoTrayecto.Origen = origen;
                    nuevoTrayecto.Destino = destino;

                    if(!camposLlenos)
                    {
                        if (trayectoDAO.InsertTrayecto(nuevoTrayecto))
                        {
                            transaccio.Commit();
                            dtgTrayectos.ItemsSource = trayectoDAO.GetTrayectos();
                            MessageBox.Show("Trayecto insertado correctamente.");
                        }
                    } else
                    {
                        MessageBox.Show("No se puede insertar un nuevo trayecto porque has seleccionado un trayecto previamente.");
                        transaccio.Rollback();
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
            using (var transaccio = _context.Database.BeginTransaction())
            {
                try
                {
                    DAOEstacion estacionDAO = new DAOEstacion(_context);
                    DAOTrayecto trayectoDAO = new DAOTrayecto(_context);

                    Estacion origen = estacionDAO.GetEstacion(txtOrigen.Text);
                    Estacion destino = estacionDAO.GetEstacion(txtDestino.Text);

                    if (origen == null || destino == null)
                    {
                        MessageBox.Show("La estación de origen o destino no existe en la base de datos.");
                        transaccio.Rollback();
                        return;
                    }

                    if (cboMedioTransporte.SelectedIndex == -1)
                    {
                        MessageBox.Show("Selecciona un medio de transporte.");
                        transaccio.Rollback();
                        return;
                    }

                    MedioTransporte medio = new MedioTransporte(cboMedioTransporte.SelectedItem.ToString());
                    medio.Id = cboMedioTransporte.SelectedIndex + 1;

                    Trayecto nuevoTrayecto = new Trayecto(txtNombre.Text, DateTime.Now, medio);
                    nuevoTrayecto.Origen = origen;
                    nuevoTrayecto.Destino = destino;

                    Trayecto trayectoAux = null;

                    trayectoAux = trayectoDAO.GetTrayecto(txtNombre.Text);

                    if(camposLlenos)
                    {
                        if (trayectoAux != null)
                        {
                            nuevoTrayecto.Id = trayectoAux.Id;
                            if (trayectoDAO.UpdateTrayecto(nuevoTrayecto))
                            {
                                transaccio.Commit();
                                dtgTrayectos.ItemsSource = trayectoDAO.GetTrayectos();
                                MessageBox.Show("Trayecto actualizado correctamente.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontró un trayecto con ese nombre para actualizar.");
                        }
                    } else
                    {
                        MessageBox.Show("No se puede modificar un trayecto porque no has seleccionado uno previamente.");
                        transaccio.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    transaccio.Rollback();
                    MessageBox.Show("Error al actualizar: " + ex.Message);
                }
            }
        }

        private void dtgParadas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
