using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
  public interface IGenericRepository<T> where T : BaseEntity
  {
    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync (ISpecification<T> spec);

    // Они не асинхронные
    // причина в том, что мы не собираемся добавлять элементы в базу данных
    // когда мы используем любой из этих методов.
    // Мы только говорим EF, что хотим добавить это, поэтому отслеживайте это (чтобы это происходило в памяти).
    // Наша единица работы отвечает за сохранение его в базу данных, а не в репозиторий.
    void Add(T entity);
    void Update (T entity);
    void Delete (T entity);

  }
}