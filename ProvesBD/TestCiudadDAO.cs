using DAO;
using Modelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProvesBD
{
    [TestClass]
    public class TestCiudadDAO
    {
        // ---------- SELECT ----------
        [TestMethod]
        public void GetCiudades_SinFiltros_Devuelve1()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOCiudad.GetCiudades("", "");
                Assert.AreEqual(10, lista.Count);
            });
        }

        [TestMethod]
        public void GetCiudad_IdExistente_DevuelveBarcelona()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var e = uow.DAOCiudad.GetCiudad(1);
                Assert.IsNotNull(e);
                Assert.AreEqual("Barcelona", e.Nombre);
            });
        }

        [TestMethod]
        public void GetCiudad_IdInexistente_DevuelveNull()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.IsNull(uow.DAOCiudad.GetCiudad(9999));
            });
        }

        [TestMethod]
        public void GetCiudades_FiltroNombre_NoDistingueMayusculas()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOCiudad.GetCiudades("", "girona");
                Assert.AreEqual(1, lista.Count);
            });
        }

        [TestMethod]
        public void GetNumeroCiudades_SinFiltros_Devuelve10()
        {
            MyOracleFactory.getUOW(uow =>
            {
                Assert.AreEqual(10, uow.DAOCiudad.GetNumeroCiutats("", ""));
            });
        }

        [TestMethod]
        public void GetCiudades_Paginacion_Devuelve3()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var lista = uow.DAOCiudad.GetCiudades("", "", 0, 3);
                Assert.AreEqual(3, lista.Count);
            });
        }

        // ---------- INSERT / UPDATE / DELETE ----------
        [TestMethod]
        public void Ciudad_CicloCompleto_InsertUpdateDelete()
        {
            MyOracleFactory.getUOW(uow =>
            {
                var dao = uow.DAOCiudad;

                // INSERT
                var nueva = new Ciudad(0, "TEST_CIUDAD");
                Assert.IsTrue(dao.InsertCiudad(nueva));
                Assert.IsTrue(nueva.Id > 0, "El insert debe devolver el id generado");

                // SELECT de lo insertado
                var leida = dao.GetCiudad(nueva.Id);
                Assert.IsNotNull(leida);
                Assert.AreEqual("TEST_CIUDAD", leida.Nombre);

                // UPDATE
                nueva.Nombre = "TEST_MODIFICADA";
                Assert.IsTrue(dao.UpdateCiudad(nueva));
                Assert.AreEqual("TEST_MODIFICADA", dao.GetCiudad(nueva.Id).Nombre);

                // DELETE
                Assert.IsTrue(dao.DeleteCiudad(nueva.Id));
                Assert.IsNull(dao.GetCiudad(nueva.Id));
            });
        }
    }
}
