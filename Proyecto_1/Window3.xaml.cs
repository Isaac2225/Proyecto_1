using DAO;
using Modelo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.RightsManagement;
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
    public partial class Window3 : Window
    {
        private readonly Window _MainWindow;
        private readonly MyDBContext _context = new MyDBContext();
        private bool _sincronizandoPwd = false;

        public Window3(Window mainWindow)
        {
            InitializeComponent();
            _MainWindow = mainWindow;
        }

        private void TxtPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_sincronizandoPwd) return;
            _sincronizandoPwd = true;
            txtPwdVisible.Text = txtPwd.Password;
            _sincronizandoPwd = false;
        }

        private void TxtPwdVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_sincronizandoPwd) return;
            _sincronizandoPwd = true;
            txtPwd.Password = txtPwdVisible.Text;
            _sincronizandoPwd = false;
        }

        private void BtnVerPwd_Changed(object sender, RoutedEventArgs e)
        {
            bool mostrar = btnVerPwd.IsChecked == true;
            txtPwd.Visibility = mostrar ? Visibility.Collapsed : Visibility.Visible;
            txtPwdVisible.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool datosCorrectos = VerificarUsuario(txtMail.Text, txtPwd.Password);

            if (datosCorrectos)
            {
                DAOUsuario usuarioDAO = new DAOUsuario(_context);
                Usuario usuario = usuarioDAO.GetUsuario(txtMail.Text, txtPwd.Password);

                switch (usuario.Tipo)
                {
                    case 'A':
                        Window4 admin = new Window4();
                        admin.Show();
                        this.Close();
                        break;
                    case 'E':
                        VentanaEditor ventanaEscojer = new VentanaEditor();
                        ventanaEscojer.Show();
                        this.Close();
                        break;
                }

                _MainWindow.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window2 window = new Window2(this);
            window.Show();
            this.Close();
        }

        private bool VerificarUsuario(string user, string pwd)
        {
            DAOUsuario usuarioDAO = new DAOUsuario(_context);

            int filasMail = usuarioDAO.GetNumeroUsuariosByUsrPwd(user, null);
            int filasPwd = usuarioDAO.GetNumeroUsuariosByUsrPwd(null, pwd);
            int filasGeneral = usuarioDAO.GetNumeroUsuariosByUsrPwd(user, pwd);

            if (filasMail == 0)
            {
                txtErrorUsr.Visibility = Visibility.Visible;

                if (filasPwd == 0)
                {
                    txtErrorPwd.Visibility = Visibility.Visible;
                }
                else
                {
                    txtErrorPwd.Visibility = Visibility.Collapsed;
                }

                return false;
            }

            if (filasPwd == 0)
            {
                if (filasMail > 0)
                {
                    txtErrorUsr.Visibility = Visibility.Collapsed;
                }

                txtErrorPwd.Visibility = Visibility.Visible;
                return false;
            }

            if (filasGeneral == 0)
            {
                txtErrorDB.Visibility = Visibility.Visible;

                return false;
            }

            return true;
        }
    }
}