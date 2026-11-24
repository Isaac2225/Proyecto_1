using Modelo;
using System.Collections.ObjectModel;

namespace IDAO
{
    public interface IDAOUsuario
    {
        Usuario GetUsuario(string mail, string pwd);

        int GetNumeroUsuariosByUsrPwd(string usr, string pwd);
        ObservableCollection<Usuario> GetUsuarios(string mail, string pwd, int offset = 0, int rows_per_page = -1);
        int GetNumeroUsuarios(string idFiltro, string nombreFiltro);
    }
}