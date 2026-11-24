using DAO;
using Modelo;

namespace ProvesBD
{
    [TestClass]
    public sealed class TestParadaDAO
    {
        // ---------- SELECT ----------
        [TestMethod]
        public void GetParadas_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOParada.GetParadas("", "");
                Assert.AreEqual(10, lista.Count);
            });
        }

        [TestMethod]
        public void GetParada_Existente_DevuelveSortidaBarcelonaSants()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var p = uow.DAOParada.GetParada(1, 1);
                Assert.IsNotNull(p);
                Assert.AreEqual("Sortida Barcelona Sants", p.Nombre);
                Assert.AreEqual(1, p.Orden);
                Assert.AreEqual(new DateTime(2026, 10, 1, 7, 30, 0), p.HoraPaso);
            });
        }

        [TestMethod]
        public void GetParada_CargaTrayecto()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var p = uow.DAOParada.GetParada(5, 1);
                Assert.IsNotNull(p);
                Assert.IsNotNull(p.Id_trayecto);
                Assert.AreEqual(5, p.Id_trayecto.Id);
            });
        }

        [TestMethod]
        public void GetParada_TrayectoInexistente_DevuelveNull()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.IsNull(uow.DAOParada.GetParada(9999, 1));
            });
        }

        [TestMethod]
        public void GetParada_OrdenInexistente_DevuelveNull()
        {
            MyOracleFactory.getUOW(uow =>
            {
                // El trayecto 1 solo tiene la parada con orden 1
                Assert.IsNull(uow.DAOParada.GetParada(1, 99));
            });
        }

        [TestMethod]
        public void GetParadas_FiltroTrayecto_DevuelveSoloEse()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOParada.GetParadas("2", "");
                Assert.AreEqual(1, lista.Count);
                Assert.AreEqual("Parada Tarragona", lista[0].Nombre);
            });
        }

        [TestMethod]
        public void GetParadas_FiltroNombre_NoDistingueMayusculas()
        {
            MyOracleFactory.getUOW(uow =>
            {
                // Trayectos 4 y 6: "Parada Badalona Centre" y "Sortida Badalona Centre"
                var lista = uow.DAOParada.GetParadas("", "badalona");
                Assert.AreEqual(2, lista.Count);
            });
        }

        [TestMethod]
        public void GetParadas_FiltroNombre_Sortida_Devuelve4()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOParada.GetParadas("", "sortida");
                Assert.AreEqual(4, lista.Count);
            });
        }

        [TestMethod]
        public void GetParadas_FiltroSinCoincidencias_DevuelveVacio()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOParada.GetParadas("", "NO_EXISTE_XYZ");
                Assert.AreEqual(0, lista.Count);
            });
        }

        // ---------- COUNT ----------
        [TestMethod]
        public void GetNumeroParadas_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(10, uow.DAOParada.GetNumeroParadas("", ""));
            });
        }

        [TestMethod]
        public void GetNumeroParadas_FiltroNombre_Parada_Devuelve6()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(6, uow.DAOParada.GetNumeroParadas("", "parada"));
            });
        }

        [TestMethod]
        public void GetNumeroParadas_FiltroTrayecto_Devuelve1()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(1, uow.DAOParada.GetNumeroParadas("7", ""));
            });
        }

        // ---------- PAGINACIÓN ----------
        [TestMethod]
        public void GetParadas_Paginacion_Devuelve3()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOParada.GetParadas("", "", 0, 3);
                Assert.AreEqual(3, lista.Count);
            });
        }

        [TestMethod]
        public void GetParadas_PaginacionSegundaPagina_EmpiezaEnTrayecto4()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOParada.GetParadas("", "", 3, 3);
                Assert.AreEqual(3, lista.Count);
                Assert.AreEqual(4, lista[0].Id_trayecto.Id);
            });
        }

        // ---------- INSERT / UPDATE / DELETE ----------
        [TestMethod]
        public void Parada_CicloCompleto_InsertUpdateDelete()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var dao = uow.DAOParada;

                // INSERT (usa un trayecto existente por la clave foránea; orden 2 para no chocar con la PK)
                var hora = new DateTime(2026, 10, 1, 16, 0, 0);
                var nueva = new Parada(2, "TEST_PARADA", hora, new Trayecto(1, "TEST", DateTime.Now, false, null, null));
                Assert.IsTrue(dao.InsertParada(nueva));

                // SELECT de lo insertado
                var leida = dao.GetParada(1, 2);
                Assert.IsNotNull(leida);
                Assert.AreEqual("TEST_PARADA", leida.Nombre);
                Assert.AreEqual(hora, leida.HoraPaso);

                // UPDATE
                nueva.Nombre = "TEST_MODIFICADA";
                Assert.IsTrue(dao.UpdateParada(nueva));
                Assert.AreEqual("TEST_MODIFICADA", dao.GetParada(1, 2).Nombre);

                // DELETE
                Assert.IsTrue(dao.DeleteParada(1, 2));
                Assert.IsNull(dao.GetParada(1, 2));
            });
        }
    }
}