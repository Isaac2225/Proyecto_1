using Capas_BD_Proyecto_1;
using IDAO;
using IDAOCiudad;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAO
{
    public class UnitOfWork : IDisposable
    {
        private readonly MyDBContext _context;

        public IDAOCiutat DAOCiudad { get; }
        public IDAOTrayecto DAOTrayecto { get; }
        public IDAOEstacion DAOEstacion { get; }
        public IDAOParada DAOParada { get;  }
        public IDAOMedioTransporte DAOMedioTransporte { get; }
        public IDAOUsuario DAOUsuario { get; }


        public UnitOfWork(MyDBContext context)
        {
            _context = context;
            DAOCiudad = new DAOCiudad(_context);
            DAOTrayecto = new DAOTrayecto(_context);
            DAOEstacion = new DAOEstacion(_context);
            DAOParada = new DAOParada(_context);
            DAOMedioTransporte = new DAOMedioTransporte(_context);
            DAOUsuario = new DAOUsuario(_context);
        }

        public void BeginTransaction() => _context.Database.BeginTransaction();
        public void Commit() => _context.Database.CurrentTransaction?.Commit();

        public void Rollback() => _context.Database.CurrentTransaction?.Rollback();

        public void Dispose()
        {
        }
    }
}
