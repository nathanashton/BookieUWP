using System.Collections.Generic;

namespace Bookie.Common.Interfaces
{
    public interface IBookieData<T>
    {
        List<T> GetAll();

        T GetSingle(int id);

        T GetSingle(string query);

        T Add(T item);

        bool Exists(T item);

        void Remove(T item);

        void Udpate(T item);
    }
}