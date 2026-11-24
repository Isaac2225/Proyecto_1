using DAO;
using Microsoft.EntityFrameworkCore;
using Modelo;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proyecto_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CargarHoras();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private readonly MyDBContext _context = new MyDBContext();



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DAOTrayecto DAOTrayecto = new DAOTrayecto(_context);
            dgTrayecto.ItemsSource = DAOTrayecto.GetTrayectosValidados(
                txtOrigen.Text,
                txtDestino.Text,
                horaSalida: TimeSpan.TryParse(cboHoraSalida.SelectedItem?.ToString(), out var hs) ? hs : (TimeSpan?)null,
                horaLlegada: TimeSpan.TryParse(cboLlegada.SelectedItem?.ToString(), out var hl) ? hl : (TimeSpan?)null);
        }

        private bool ignorarCambio = false;
        private void txtOrigen_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ignorarCambio)
            {
                return;
            }

            DAOEstacion dAOEstacion = new DAOEstacion(_context);
            string buscar = txtOrigen.Text.Trim();

            if (buscar.Length >= 2)
            {
                ObservableCollection<Estacion> resultados = dAOEstacion.GetEstaciones(txtOrigen.Text);

                if (resultados.Count > 0)

                {
                    lstSugerencias.ItemsSource = resultados;
                    popupSugerencias.IsOpen = true;
                }
                else
                {
                    popupSugerencias.IsOpen = false;
                }

            }
            else
            {
                popupSugerencias.IsOpen = false;
            }
        }

        private void lstSugerencias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verificamos que se haya seleccionado un elemento válido
            if (lstSugerencias.SelectedItem is Estacion estacionSeleccionada)
            {
                ignorarCambio = true; // Apagamos temporalmente el evento TextChanged para no hacer una búsqueda inútil

                // Autocompletamos el TextBox con el nombre de la estación
                txtOrigen.Text = estacionSeleccionada.Nombre;

                // Movemos el cursor al final de la palabra que se acaba de autocompletar
                txtOrigen.CaretIndex = txtOrigen.Text.Length;

                // Ocultamos la lista de sugerencias
                popupSugerencias.IsOpen = false;

                ignorarCambio = false; // Volvemos a encender el evento TextChanged
            }
        }

        private void Button_Click_info(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Trayecto t)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"ID Trayecto: {t.Id}");
                sb.AppendLine($"Nombre: {t.Nombre}");
                sb.AppendLine($"Fecha de creación: {t.FechaCreacion:dd/MM/yyyy HH:mm}");
                sb.AppendLine($"Validado: {(t.Validado ? "Sí" : "No")}");
                sb.AppendLine("----------------------------------------");

                sb.AppendLine($"Medio de transporte: {t.Medio?.Nombre ?? "No especificado"}");
                sb.AppendLine($"Origen: {t.Origen?.Nombre ?? "No especificado"}");
                sb.AppendLine($"Destino: {t.Destino?.Nombre ?? "No especificado"}");

                tbDetalleTrayecto.Text = sb.ToString();

                dgTrayecto.Visibility = Visibility.Collapsed;
                panelDetalle.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click_Volver(object sender, RoutedEventArgs e)
        {
            dgTrayecto.Visibility = Visibility.Visible;
            panelDetalle.Visibility = Visibility.Collapsed;
        }

        private void lstSugerencias2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSugerencias2.SelectedItem is Estacion estacionSeleccionada)
            {
                ignorarCambio = true;


                txtDestino.Text = estacionSeleccionada.Nombre;

                txtDestino.CaretIndex = txtDestino.Text.Length;

                popupSugerencias2.IsOpen = false;

                ignorarCambio = false;
            }
        }

        private void txtDestino_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ignorarCambio)
            {
                return;
            }

            DAOEstacion dAOEstacion = new DAOEstacion(_context);
            string buscar = txtDestino.Text.Trim();

            if (buscar.Length >= 2)
            {
                ObservableCollection<Estacion> resultados = dAOEstacion.GetEstaciones(txtDestino.Text);

                if (resultados.Count > 0)

                {
                    lstSugerencias2.ItemsSource = resultados;
                    popupSugerencias2.IsOpen = true;
                }
                else
                {
                    popupSugerencias2.IsOpen = false;
                }

            }
            else
            {
                popupSugerencias2.IsOpen = false;
            }
        }

        private void CargarHoras()
        {
            var horas = new List<string>();

            for (int h = 0; h < 24; h++)
            {
                for (int m = 0; m < 60; m += 15)
                {
                    horas.Add($"{h:00}:{m:00}");
                }
            }

            cboHoraSalida.ItemsSource = horas;
            cboLlegada.ItemsSource = horas;
        }

        private void Button_Click_Log(object sender, RoutedEventArgs e)
        {
            Window3 login = new Window3();
            login.Show();
            
        }

        private void Button_Click_Sign(object sender, RoutedEventArgs e)
        {
            Window2 signin = new Window2();
            signin.Show();
            this.Close();
        }
    }
}