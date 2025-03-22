using System.Collections.ObjectModel;

namespace NETX.Services.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        ObservableCollection<T> GetAll();

        T? Get(string id);

        bool Contains(T entity);

        T Add(T entity);

        Task<T> AddAsync(T entity);

        T Update(T entity);

        Task<T> UpdateAsync(T entity);

        void Remove(string id);

        Task RemoveAsync(string id);
    }
}
