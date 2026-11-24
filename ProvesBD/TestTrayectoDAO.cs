using DAO;
using Modelo;

namespace ProvesBD
{
    [TestClass]
    public sealed class TestTrayectoDAO
    {
        // ---------- SELECT ----------
        [TestMethod]
        public void GetTrayectos_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOTrayecto.GetTrayectos("", "");
                Assert.AreEqual(10, lista.Count);
            });
        }

        [TestMethod]
        public void GetTrayecto_IdExistente_DevuelveBarcelonaGirona()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var t = uow.DAOTrayecto.GetTrayecto(1);
                Assert.IsNotNull(t);
                Assert.AreEqual("Barcelona - Girona Exprés", t.Nombre);
                Assert.AreEqual(new DateTime(2023, 7, 1), t.FechaCreacion);
                Assert.IsTrue(t.Validado);
            });
        }

        [TestMethod]
        public void GetTrayecto_NoValidado_DevuelveValidadoFalse()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var t = uow.DAOTrayecto.GetTrayecto(3);
                Assert.IsNotNull(t);
                Assert.AreEqual("Lleida - Manresa Rural", t.Nombre);
                Assert.IsFalse(t.Validado);
            });
        }

        [TestMethod]
        public void GetTrayecto_CargaUsuarioYMedio()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var t = uow.DAOTrayecto.GetTrayecto(3);
                Assert.IsNotNull(t);
                Assert.IsNotNull(t.Id_usuario);
                Assert.IsNotNull(t.Medio);
                Assert.AreEqual(3, t.Id_usuario.Id);
                Assert.AreEqual(6, t.Medio.Id);
            });
        }

        [TestMethod]
        public void GetTrayecto_IdInexistente_DevuelveNull()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.IsNull(uow.DAOTrayecto.GetTrayecto(9999));
            });
        }

        [TestMethod]
        public void GetTrayectos_FiltroNombre_NoDistingueMayusculas()
        {
            MyOracleFactory.getUOW(uow =>
            {
                // "Barcelona - Girona Exprés" y "Vic - Girona Interior"
                var lista = uow.DAOTrayecto.GetTrayectos("", "girona");
                Assert.AreEqual(2, lista.Count);
            });
        }

        [TestMethod]
        public void GetTrayectos_FiltroId_DevuelveSoloEse()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOTrayecto.GetTrayectos("2", "");
                Assert.AreEqual(1, lista.Count);
                Assert.AreEqual("Ruta Costa Daurada", lista[0].Nombre);
            });
        }

        [TestMethod]
        public void GetTrayectos_FiltroSinCoincidencias_DevuelveVacio()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOTrayecto.GetTrayectos("", "NO_EXISTE_XYZ");
                Assert.AreEqual(0, lista.Count);
            });
        }

        // ---------- COUNT ----------
        [TestMethod]
        public void GetNumeroTrayectos_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(10, uow.DAOTrayecto.GetNumeroTrayectos("", ""));
            });
        }

        [TestMethod]
        public void GetNumeroTrayectos_FiltroNombre_Devuelve2()
        {
            MyOracleFactory.getUOW(uow =>
            {
                // "Ruta Costa Daurada" y "Ruta del Bages"
                Assert.AreEqual(2, uow.DAOTrayecto.GetNumeroTrayectos("", "ruta"));
            });
        }

        // ---------- PAGINACIÓN ----------
        [TestMethod]
        public void GetTrayectos_Paginacion_Devuelve3()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOTrayecto.GetTrayectos("", "", 0, 3);
                Assert.AreEqual(3, lista.Count);
            });
        }

        [TestMethod]
        public void GetTrayectos_PaginacionSegundaPagina_EmpiezaEnId4()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOTrayecto.GetTrayectos("", "", 3, 3);
                Assert.AreEqual(3, lista.Count);
                Assert.AreEqual(4, lista[0].Id);
            });
        }
    }
}