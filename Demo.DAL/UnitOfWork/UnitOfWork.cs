using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Demo.DAL.Contracts;
using Demo.Entities;

namespace Demo.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, IRepository> _repositories;

        public UnitOfWork(/* IRepository<ModelName> ModelNameRepo */
            IRepository<Users> UserRepo)
        {
            _repositories = _repositories ?? new Dictionary<Type, IRepository>();

            //  _repositories.Add(typeof(ModelName), ModelNameRepo);
            _repositories.Add(typeof(Users), UserRepo);
        }
        public IRepository GetRepository<T>() where T : class
        {
            return _repositories[typeof(T)];
        }
        public void Commit()
        {
            _repositories.ToList().ForEach(x => x.Value.Submit());
        }


        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }
    }
}
