using DAO;
using Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ProvesBD
{
    [TestClass]
    public sealed class TestUsuarioDAO
    {
        // ---------- SELECT ----------
        [TestMethod]
        public void GetUsuarios_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOUsuario.GetUsuarios("", "");
                Assert.AreEqual(10, lista.Count);
            });
        }

        [TestMethod]
        public void GetUsuario_IdExistente_DevuelveDatosCorrectos()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var u = uow.DAOUsuario.GetUsuario(1);
                Assert.IsNotNull(u);

                Assert.AreEqual(1, u.Id);
                Assert.AreEqual("marc.puig@example.com", u.Email);
                Assert.AreEqual("Marc Puig", u.Nombre);
                Assert.AreEqual('A', u.Tipo);
                Assert.AreEqual(new DateTime(2023, 1, 15), u.FechaRegistro);
            });
        }

        [TestMethod]
        public void GetUsuario_IdInexistente_DevuelveNull()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.IsNull(uow.DAOUsuario.GetUsuario(9999));
            });
        }

        [TestMethod]
        public void GetUsuarios_FiltroNombre_NoDistingueMayusculas()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOUsuario.GetUsuarios("", "puig");
                Assert.IsTrue(lista.Count > 0);
            });
        }

        [TestMethod]
        public void GetUsuarios_FiltroId_DevuelveSoloEse()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOUsuario.GetUsuarios("2", "");
                Assert.AreEqual(1, lista.Count);
                Assert.AreEqual(2, lista[0].Id);
            });
        }

        [TestMethod]
        public void GetUsuarios_FiltroSinCoincidencias_DevuelveVacio()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOUsuario.GetUsuarios("", "NO_EXISTE_XYZ");
                Assert.AreEqual(0, lista.Count);
            });
        }

        // ---------- COUNT ----------
        [TestMethod]
        public void GetNumeroUsuarios_SinFiltros_DevuelveTotal()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(10, uow.DAOUsuario.GetNumeroUsuarios("", ""));
            });
        }

        [TestMethod]
        public void GetNumeroUsuarios_FiltroNombre_DevuelveCantidadCorrecta()
        {
            MyOracleFactory.getUOW(uow =>
            {
                // Busca "jo" (coincidirá con Jordi Ferrer y Meritxell Jové)
                Assert.AreEqual(2, uow.DAOUsuario.GetNumeroUsuarios("", "jo"));
            });
        }

        // ---------- PAGINACIÓN ----------
        [TestMethod]
        public void GetUsuarios_Paginacion_DevuelveCantidadIndicada()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOUsuario.GetUsuarios("", "", 0, 3);
                Assert.AreEqual(3, lista.Count);
            });
        }

        [TestMethod]
        public void GetUsuarios_PaginacionSegundaPagina_EmpiezaEnOffsetCorrecto()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOUsuario.GetUsuarios("", "", 3, 3);
                Assert.AreEqual(3, lista.Count);
                Assert.AreEqual(4, lista[0].Id);
            });
        }
    }
}