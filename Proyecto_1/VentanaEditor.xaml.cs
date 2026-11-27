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
    public partial class VentanaEditor : Window
    {
        private bool camposLlenosTrayecto = false;
        private bool camposLlenosParada = false;
        private readonly MyDBContext _context;

        public VentanaEditor()
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

        private void Button_Click_Eliminar_Trayectos(object sender, RoutedEventArgs e)
        {
            Trayecto selectedTrayecto = (sender as FrameworkElement)?.DataContext as Trayecto
                ?? dtgTrayectos.SelectedItem as Trayecto;

            if (selectedTrayecto != null)
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
                            txtOrigen.Text = "";
                            txtDestino.Text = "";
                            txtNombre.Text = "";
                            camposLlenosTrayecto = false;
                        }
                        else
                        {
                            transaccion.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        try { transaccion.Rollback(); } catch { }
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
            camposLlenosTrayecto = true;
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

                    if (!camposLlenosTrayecto)
                    {
                        if (trayectoDAO.InsertTrayecto(nuevoTrayecto))
                        {
                            transaccio.Commit();
                            dtgTrayectos.ItemsSource = trayectoDAO.GetTrayectos();
                            MessageBox.Show("Trayecto insertado correctamente.");
                        }
                    }
                    else
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

                    Trayecto trayectoAux = trayectoDAO.GetTrayecto(txtNombre.Text);

                    if (camposLlenosTrayecto)
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
                    }
                    else
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

        private void dtgParadas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtgParadas.SelectedItem is Parada selectedParada)
            {
                txtHoraLlegada.Text = selectedParada.HoraLlegada?.ToString("HH:mm") ?? "";
                txtHoraSalida.Text = selectedParada.HoraSalida?.ToString("HH:mm") ?? "";
            }

            camposLlenosParada = true;
        }

        private void Button_Click_Eliminar_Paradas(object sender, RoutedEventArgs e)
        {
            Parada selectedParada = (sender as FrameworkElement)?.DataContext as Parada
                ?? dtgParadas.SelectedItem as Parada;

            if (selectedParada != null)
            {
                using (var transaccion = _context.Database.BeginTransaction())
                {
                    try
                    {
                        DAOParada paradaDAO = new DAOParada(_context);
                        bool eliminado = paradaDAO.DeleteParada(selectedParada.Id_trayecto?.Id, selectedParada.Orden);

                        if (eliminado)
                        {
                            transaccion.Commit();
                            CargarDataGrids();
                            txtHoraLlegada.Text = "";
                            txtHoraSalida.Text = "";
                            camposLlenosParada = false;
                        }
                        else
                        {
                            transaccion.Rollback();
                            MessageBox.Show("No se encontró la parada en la base de datos para eliminar.");
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
                MessageBox.Show("Seleccione una parada para eliminar.");
            }
        }

        private void Button_Click_Insertar_Paradas(object sender, RoutedEventArgs e)
        {
            if (camposLlenosParada)
            {
                MessageBox.Show("No se puede insertar una nueva parada porque has seleccionado una parada previamente.");
                return;
            }

            if (!PedirDatosParada(out Trayecto trayectoElegido, out string nombreParada, out string nombreEstacion))
            {
                return;
            }

            using (var transaccio = _context.Database.BeginTransaction())
            {
                try
                {
                    DAOEstacion estacionDAO = new DAOEstacion(_context);
                    Estacion estacion = estacionDAO.GetEstacion(nombreEstacion);

                    if (estacion == null)
                    {
                        MessageBox.Show($"La estación \"{nombreEstacion}\" no existe en la base de datos.");
                        transaccio.Rollback();
                        return;
                    }

                    DateTime? horaLlegada = string.IsNullOrWhiteSpace(txtHoraLlegada.Text)
                        ? null
                        : DateTime.ParseExact(txtHoraLlegada.Text.Trim(), "HH:mm", null);

                    DateTime? horaSalida = string.IsNullOrWhiteSpace(txtHoraSalida.Text)
                        ? null
                        : DateTime.ParseExact(txtHoraSalida.Text.Trim(), "HH:mm", null);

                    if (horaLlegada == null && horaSalida == null)
                    {
                        MessageBox.Show("Debes indicar al menos la hora de llegada o la de salida.");
                        transaccio.Rollback();
                        return;
                    }

                    DAOParada paradaDAO = new DAOParada(_context);
                    Parada nuevaParada = new Parada(trayectoElegido, estacion.Id, 0, nombreParada, horaLlegada, horaSalida);

                    if (paradaDAO.InsertParada(nuevaParada))
                    {
                        transaccio.Commit();
                        dtgParadas.ItemsSource = paradaDAO.GetParadas();
                        MessageBox.Show("Parada insertada correctamente.");
                    }
                    else
                    {
                        transaccio.Rollback();
                        MessageBox.Show("No se pudo insertar la parada.");
                    }
                }
                catch (Exception ex)
                {
                    transaccio.Rollback();
                    MessageBox.Show("Error al insertar: " + ex.Message);
                }
            }
        }

        private bool PedirDatosParada(out Trayecto trayectoElegido, out string nombreParada, out string nombreEstacion)
        {
            trayectoElegido = null;
            nombreParada = "";
            nombreEstacion = "";

            DAOTrayecto trayectoDAO = new DAOTrayecto(_context);
            var trayectos = trayectoDAO.GetTrayectos();

            Window dialogo = new Window
            {
                Title = "Nueva parada",
                Width = 360,
                Height = 290,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E2126")),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB")),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13
            };

            var inputBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#262A30"));
            var borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323741"));
            var textBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB"));

            StackPanel panel = new StackPanel { Margin = new Thickness(18) };

            panel.Children.Add(new TextBlock { Text = "Trayecto (opcional):", Margin = new Thickness(0, 0, 0, 4), Foreground = textBrush });
            ComboBox cboTrayecto = new ComboBox
            {
                Height = 30,
                Margin = new Thickness(0, 0, 0, 12),
                ItemsSource = trayectos,
                DisplayMemberPath = "Nombre"
            };
            panel.Children.Add(cboTrayecto);

            panel.Children.Add(new TextBlock { Text = "Nombre de la parada:", Margin = new Thickness(0, 0, 0, 4), Foreground = textBrush });
            TextBox txtNomParada = new TextBox
            {
                Height = 30,
                Margin = new Thickness(0, 0, 0, 12),
                Padding = new Thickness(6, 4, 6, 4),
                Background = inputBg,
                Foreground = textBrush,
                CaretBrush = textBrush,
                BorderBrush = borderBrush
            };
            panel.Children.Add(txtNomParada);

            panel.Children.Add(new TextBlock { Text = "Nombre de la estación:", Margin = new Thickness(0, 0, 0, 4), Foreground = textBrush });
            TextBox txtNomEstacion = new TextBox
            {
                Height = 30,
                Margin = new Thickness(0, 0, 0, 16),
                Padding = new Thickness(6, 4, 6, 4),
                Background = inputBg,
                Foreground = textBrush,
                CaretBrush = textBrush,
                BorderBrush = borderBrush
            };
            panel.Children.Add(txtNomEstacion);

            StackPanel botones = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

            Button btnCancelar = new Button
            {
                Content = "Cancelar",
                Width = 85,
                Height = 30,
                Margin = new Thickness(0, 0, 8, 0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                IsCancel = true
            };

            Button btnAceptar = new Button
            {
                Content = "Aceptar",
                Width = 85,
                Height = 30,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                IsDefault = true
            };

            btnAceptar.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(txtNomParada.Text) || string.IsNullOrWhiteSpace(txtNomEstacion.Text))
                {
                    MessageBox.Show("Rellena el nombre de la parada y el de la estación.");
                    return;
                }
                dialogo.DialogResult = true;
            };

            botones.Children.Add(btnCancelar);
            botones.Children.Add(btnAceptar);
            panel.Children.Add(botones);

            dialogo.Content = panel;

            if (dialogo.ShowDialog() == true)
            {
                trayectoElegido = cboTrayecto.SelectedItem as Trayecto;
                nombreParada = txtNomParada.Text.Trim();
                nombreEstacion = txtNomEstacion.Text.Trim();
                return true;
            }

            return false;
        }

        private void Button_Click_Modificar_Paradas(object sender, RoutedEventArgs e)
        {
            if (dtgParadas.SelectedItem is not Parada paradaSeleccionada)
            {
                MessageBox.Show("Selecciona primero una parada de la tabla.");
                return;
            }

            using (var transaccio = _context.Database.BeginTransaction())
            {
                try
                {
                    DateTime? horaLlegada = string.IsNullOrWhiteSpace(txtHoraLlegada.Text)
                        ? null
                        : DateTime.ParseExact(txtHoraLlegada.Text.Trim(), "H:mm", null);

                    DateTime? horaSalida = string.IsNullOrWhiteSpace(txtHoraSalida.Text)
                        ? null
                        : DateTime.ParseExact(txtHoraSalida.Text.Trim(), "H:mm", null);

                    if (horaLlegada == null && horaSalida == null)
                    {
                        MessageBox.Show("Debes indicar al menos la hora de llegada o la de salida.");
                        transaccio.Rollback();
                        return;
                    }

                    DAOParada paradaDAO = new DAOParada(_context);

                    paradaSeleccionada.HoraLlegada = horaLlegada;
                    paradaSeleccionada.HoraSalida = horaSalida;

                    if (paradaDAO.UpdateParada(paradaSeleccionada))
                    {
                        transaccio.Commit();
                        dtgParadas.ItemsSource = paradaDAO.GetParadas();
                        MessageBox.Show("Parada actualizada correctamente.");
                    }
                    else
                    {
                        transaccio.Rollback();
                        MessageBox.Show("No se pudo actualizar la parada.");
                    }
                }
                catch (Exception ex)
                {
                    transaccio.Rollback();
                    MessageBox.Show("Error al actualizar: " + ex.Message);
                }
            }
        }

        private void Button_Click_Limpiar_Campos_Parada(object sender, RoutedEventArgs e)
        {
            txtHoraLlegada.Text = "";
            txtHoraSalida.Text = "";
            camposLlenosParada = false;
        }

        private void Button_Click_Limpiar_Campos_Trayecto(object sender, RoutedEventArgs e)
        {
            txtNombre.Text = "";
            txtOrigen.Text = "";
            txtDestino.Text = "";
            cboMedioTransporte.SelectedItem = null;
            camposLlenosTrayecto = false;
        }

        private void Button_Click_LogOut(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click_CambiarUsuario(object sender, RoutedEventArgs e)
        {
            Window3 login = new Window3(this);
            login.Show();
        }
    }
}