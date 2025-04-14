using Agendai.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agendai.Data.Database.Repositories
{
    public class ShiftRepository
    {
        private readonly AppDbContext _context;

        public ShiftRepository()
        {
            _context = new AppDbContext();
        }

        // Obter todos os shifts
        public List<Shift> GetAllShifts()
        {
            return _context.Shifts.Include(s => s.Todo).ToList();
        }

        // Obter shifts de uma tarefa específica
        public List<Shift> GetShiftsByTodoId(ulong todoId)
        {
            return _context.Shifts
                .Where(s => s.TodoId == todoId)
                .ToList();
        }

        // Adicionar um novo shift
        public bool AddShift(Shift newShift)
        {
            try
            {
                _context.Shifts.Add(newShift);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Atualizar um shift existente
        public bool UpdateShift(Shift updatedShift)
        {
            try
            {
                _context.Shifts.Update(updatedShift);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Excluir um shift
        public bool DeleteShift(ulong id)
        {
            try
            {
                var shiftToDelete = _context.Shifts.Find(id);
                if (shiftToDelete == null)
                    return false;
                    
                _context.Shifts.Remove(shiftToDelete);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}