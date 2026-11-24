using DAO;
using Modelo;

namespace ProvesBD
{
    [TestClass]
    public sealed class TestMedioTransporteDAO
    {
        // ---------- SELECT ----------
        [TestMethod]
        public void GetMediosTransporte_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOMedioTransporte.GetMediosTransporte("", "");
                Assert.AreEqual(10, lista.Count);
            });
        }

        [TestMethod]
        public void GetMedioTransporte_IdExistente_DevuelveMedio()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var m = uow.DAOMedioTransporte.GetMedioTransporte(1);
                Assert.IsNotNull(m);
                Assert.AreEqual(1, m.Id);
                Assert.IsFalse(string.IsNullOrEmpty(m.Nombre));
            });
        }

        [TestMethod]
        public void GetMedioTransporte_IdInexistente_DevuelveNull()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.IsNull(uow.DAOMedioTransporte.GetMedioTransporte(9999));
            });
        }

        [TestMethod]
        public void GetMediosTransporte_FiltroId_DevuelveSoloEse()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOMedioTransporte.GetMediosTransporte("3", "");
                Assert.AreEqual(1, lista.Count);
                Assert.AreEqual(3, lista[0].Id);
            });
        }

        [TestMethod]
        public void GetMediosTransporte_FiltroNombre_NoDistingueMayusculas()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var m = uow.DAOMedioTransporte.GetMedioTransporte(1);
                var lista = uow.DAOMedioTransporte.GetMediosTransporte("", m.Nombre.ToUpper());
                Assert.IsTrue(lista.Count >= 1);
            });
        }

        [TestMethod]
        public void GetMediosTransporte_FiltroSinCoincidencias_DevuelveVacio()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOMedioTransporte.GetMediosTransporte("", "NO_EXISTE_XYZ");
                Assert.AreEqual(0, lista.Count);
            });
        }

        // ---------- COUNT ----------
        [TestMethod]
        public void GetNumeroMediosTransporte_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(10, uow.DAOMedioTransporte.GetNumeroMediosTransporte("", ""));
            });
        }

        [TestMethod]
        public void GetNumeroMediosTransporte_FiltroId_Devuelve1()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(1, uow.DAOMedioTransporte.GetNumeroMediosTransporte("5", ""));
            });
        }

        // ---------- PAGINACIÓN ----------
        [TestMethod]
        public void GetMediosTransporte_Paginacion_Devuelve3()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOMedioTransporte.GetMediosTransporte("", "", 0, 3);
                Assert.AreEqual(3, lista.Count);
            });
        }

        [TestMethod]
        public void GetMediosTransporte_PaginacionSegundaPagina_EmpiezaEnId4()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOMedioTransporte.GetMediosTransporte("", "", 3, 3);
                Assert.AreEqual(3, lista.Count);
                Assert.AreEqual(4, lista[0].Id);
            });
        }
    }
}