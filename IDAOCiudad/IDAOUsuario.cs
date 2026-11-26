using Modelo;
using System.Collections.ObjectModel;

namespace IDAO
{
    public interface IDAOUsuario
    {
        Usuario GetUsuario(string mail, string pwd);

        int GetNumeroUsuariosByUsrPwd(string usr, string pwd);
        ObservableCollection<Usuario> GetUsuarios(
    string email = null,
    string pwd = null,
    bool? validado = null,
    int offset = 0,
    int rows_per_page = -1);
        int GetNumeroUsuarios(string idFiltro, string nombreFiltro);
        bool UpdateUsuario(Usuario u);
    }
}