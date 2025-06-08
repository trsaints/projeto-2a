using Agendai.Data.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Agendai.Data.Repositories.DesignTime;

public class TodoDesignTimeRepository : DesignTimeRepository<Todo>
{
    private readonly List<Todo> _mockTodos =
        [
            new Todo(1, "Tarefa de exemplo 1") 
            {
                Description = "Descrição 1",
                Status = TodoStatus.Incomplete, ListName = "Pessoal" 
            },
            new Todo(2, "Tarefa de exemplo 2") 
            { 
                Description = "Descrição 2", 
                Status = TodoStatus.Complete, ListName = "Trabalho" 
            }
        ];

    public override Task<Todo?> Read(int id) =>
        Task.FromResult(_mockTodos.FirstOrDefault(t => t.Id == (ulong)id));

    public override Task<IEnumerable<Todo>?> ReadByName(string name) =>
        Task.FromResult<IEnumerable<Todo>?>(_mockTodos.Where(t => t.Name.Contains(name)));

    public override Task<IEnumerable<Todo>?> ReadAll() =>
        Task.FromResult<IEnumerable<Todo>?>(_mockTodos);

    public override Task<Todo?> Create(Todo entity)
    {
        _mockTodos.Add(entity);
        return Task.FromResult<Todo?>(entity);
    }

    public override Task<Todo?> Update(Todo entity)
    {
        var idx = _mockTodos.FindIndex(t => t.Id == entity.Id);
        if (idx >= 0)
            _mockTodos[idx] = entity;
        return Task.FromResult<Todo?>(entity);
    }

    public override Task<int?> Delete(int id)
    {
        var removed = _mockTodos.RemoveAll(t => t.Id == (ulong)id);
        return Task.FromResult<int?>(removed);
    }

    public override Task<bool> Exists(Todo entity) =>
        Task.FromResult(_mockTodos.Exists(t => t.Id == entity.Id));

    public override Task<IEnumerable<Todo>?> Search(Expression<Func<Todo, bool>> predicate)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<Todo>?>(_mockTodos.Where(t => compiled(t)));
    }
}
