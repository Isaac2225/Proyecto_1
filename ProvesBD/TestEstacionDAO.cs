using DAO;
using Modelo;

namespace ProvesBD
{
    [TestClass]
    public sealed class TestEstacionDAO
    {
        // ---------- SELECT ----------
        [TestMethod]
        public void GetEstaciones_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOEstacion.GetEstaciones("", "");
                Assert.AreEqual(10, lista.Count);
            });
        }

        [TestMethod]
        public void GetEstacion_IdExistente_DevuelveBarcelonaSants()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var e = uow.DAOEstacion.GetEstacion(1);
                Assert.IsNotNull(e);
                Assert.AreEqual("Barcelona Sants", e.Nombre);
            });
        }

        [TestMethod]
        public void GetEstacion_IdInexistente_DevuelveNull()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.IsNull(uow.DAOEstacion.GetEstacion(9999));
            });
        }

        [TestMethod]
        public void GetEstaciones_FiltroNombre_NoDistingueMayusculas()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOEstacion.GetEstaciones("", "girona");
                Assert.AreEqual(1, lista.Count);
            });
        }

        [TestMethod]
        public void GetNumeroEstaciones_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(10, uow.DAOEstacion.GetNumeroEstaciones("", ""));
            });
        }

        [TestMethod]
        public void GetEstaciones_Paginacion_Devuelve3()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOEstacion.GetEstaciones("", "", 0, 3);
                Assert.AreEqual(3, lista.Count);
            });
        }

        // ---------- INSERT / UPDATE / DELETE ----------
        [TestMethod]
        public void Estacion_CicloCompleto_InsertUpdateDelete()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var dao = uow.DAOEstacion;

                // INSERT (usa una ciudad que ya exista por la clave foránea)
                var nueva = new Estacion(0, "TEST_ESTACION", 41.0, 2.0, new Ciudad(1, ""));
                Assert.IsTrue(dao.InsertEstacion(nueva));
                Assert.IsTrue(nueva.Id > 0, "El insert debe devolver el id generado");

                // SELECT de lo insertado
                var leida = dao.GetEstacion(nueva.Id);
                Assert.IsNotNull(leida);
                Assert.AreEqual("TEST_ESTACION", leida.Nombre);

                // UPDATE
                nueva.Nombre = "TEST_MODIFICADA";
                Assert.IsTrue(dao.UpdateEstacion(nueva));
                Assert.AreEqual("TEST_MODIFICADA", dao.GetEstacion(nueva.Id).Nombre);

                // DELETE
                Assert.IsTrue(dao.DeleteEstacion(nueva.Id));
                Assert.IsNull(dao.GetEstacion(nueva.Id));
            });
        }
    }
}
