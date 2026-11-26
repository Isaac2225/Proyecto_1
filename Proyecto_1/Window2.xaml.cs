using DAO;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private readonly Window _MainWindow;
        public Window2(Window mainWindow)
        {
            InitializeComponent();
            _MainWindow = mainWindow;
        }

        private readonly MyDBContext _context = new MyDBContext();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DAOUsuario usuarioDAO = new DAOUsuario(_context);

            if (!PwdCoinciden(txtPwd.Password, txtConfirmPwd.Password))
            {
                txtError2Pwd.Visibility = Visibility.Visible;
                return;
            }
            txtError2Pwd.Visibility = Visibility.Collapsed;

            if (!EmailValido(txtMail.Text))
            {
                txtErrorMail.Visibility = Visibility.Visible;
                return;
            }
            txtErrorMail.Visibility = Visibility.Collapsed;

            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    if (usuarioDAO.insertUsuario(txtNombre.Text, txtMail.Text, txtPwd.Password, DateTime.Now))
                    {
                        MainWindow window = new MainWindow();
                        window.Show();
                        this.Close();
                        tx.Commit();
                    }
                    else
                    {
                        tx.Rollback();
                    }
                }
                catch (OracleException ex) when (ex.Number == 1)
                {
                    tx.Rollback();
                    txtErrorMail.Text = "Ese email ya está registrado";
                    txtErrorMail.Visibility = Visibility.Visible;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            _MainWindow.Close();
        }

        private bool _sincronizando = false;

        private void BtnVerPwd_Changed(object sender, RoutedEventArgs e)
        {
            bool mostrar = btnVerPwd.IsChecked == true;

            txtPwdVisible.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
            txtPwd.Visibility = mostrar ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TxtPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_sincronizando) return;
            _sincronizando = true;
            txtPwdVisible.Text = txtPwd.Password;
            _sincronizando = false;
        }

        private void TxtPwdVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_sincronizando) return;
            _sincronizando = true;
            txtPwd.Password = txtPwdVisible.Text;
            _sincronizando = false;
        }

        private bool _sincronizandoConfirm = false;

        private void BtnVerConfirmPwd_Changed(object sender, RoutedEventArgs e)
        {
            bool mostrar = btnVerConfirmPwd.IsChecked == true;

            txtConfirmPwdVisible.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
            txtConfirmPwd.Visibility = mostrar ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TxtConfirmPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_sincronizandoConfirm) return;
            _sincronizandoConfirm = true;
            txtConfirmPwdVisible.Text = txtConfirmPwd.Password;
            _sincronizandoConfirm = false;
        }

        private void TxtConfirmPwdVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_sincronizandoConfirm) return;
            _sincronizandoConfirm = true;
            txtConfirmPwd.Password = txtConfirmPwdVisible.Text;
            _sincronizandoConfirm = false;
        }


        private bool PwdCoinciden (string pwd, string confirmPwd)
        {
            if(pwd.Equals(confirmPwd))
            {
                return true;
            }

            return false;
        }

        public static bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
    }
}
