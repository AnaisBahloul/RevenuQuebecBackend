using System.Collections.Generic;

namespace RevenuQuebec.SharedKernel.Interfaces
{
    public interface IRepository<T> where T : BaseEntity, IAggregateRoot
    {
        T GetById<T>(int id) where T : BaseEntity;
        List<T> List<T>() where T : BaseEntity;
        T Add<T>(T entity) where T : BaseEntity;
        void Update<T>(T entity) where T : BaseEntity;
        void Delete<T>(T entity) where T : BaseEntity;
    }
}
