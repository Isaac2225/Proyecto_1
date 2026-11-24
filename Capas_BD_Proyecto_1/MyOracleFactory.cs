using System;
using System.Collections.Generic;
using System.Text;

namespace DAO
{
    public class MyOracleFactory
    {
        public static void getUOW(Action<UnitOfWork> action)
        {
            using (var uow = new UnitOfWork(new MyDBContext()))
            {
                uow.BeginTransaction();
                action(uow);

            }
        }
    }
}
