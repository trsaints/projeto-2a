using Agendai.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agendai.Data.Database.Repositories
{
    public class TodoRepository
    {
        private readonly AppDbContext _context;

        public TodoRepository()
        {
            _context = new AppDbContext();
        }

        // Obter todas as tarefas
        public List<Todo> GetAllTodos()
        {
            return _context.Todos.ToList();
        }

        // Obter tarefas por status
        public List<Todo> GetTodosByStatus(TodoStatus status)
        {
            return _context.Todos.Where(t => t.Status == status).ToList();
        }

        // Obter uma tarefa por ID
        public Todo GetTodoById(ulong id)
        {
            return _context.Todos.Find(id);
        }

        // Obter tarefas por nome da lista
        public List<Todo> GetTodosByListName(string listName)
        {
            return _context.Todos.Where(t => t.ListName == listName).ToList();
        }

        // Adicionar uma nova tarefa
        public bool AddTodo(Todo newTodo)
        {
            try
            {
                _context.Todos.Add(newTodo);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Atualizar uma tarefa existente
        public bool UpdateTodo(Todo updatedTodo)
        {
            try
            {
                _context.Todos.Update(updatedTodo);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Atualizar apenas o status de uma tarefa
        public bool UpdateTodoStatus(ulong id, TodoStatus newStatus)
        {
            try
            {
                var todo = _context.Todos.Find(id);
                if (todo == null)
                    return false;
                
                todo.Status = newStatus;
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Excluir uma tarefa
        public bool DeleteTodo(ulong id)
        {
            try
            {
                var todoToDelete = _context.Todos.Find(id);
                if (todoToDelete == null)
                    return false;
                    
                _context.Todos.Remove(todoToDelete);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Obter tarefas com data de vencimento próxima (dentro de X dias)
        public List<Todo> GetUpcomingTodos(int days)
        {
            var dateLimit = DateTime.Now.AddDays(days);
            return _context.Todos
                .Where(t => t.Due <= dateLimit && t.Status == TodoStatus.Incomplete)
                .OrderBy(t => t.Due)
                .ToList();
        }

        // Marcar tarefa como concluída
        public bool MarkTodoAsComplete(ulong id)
        {
            return UpdateTodoStatus(id, TodoStatus.Complete);
        }

        // Marcar tarefa como pulada
        public bool MarkTodoAsSkipped(ulong id)
        {
            return UpdateTodoStatus(id, TodoStatus.Skipped);
        }
    }
}