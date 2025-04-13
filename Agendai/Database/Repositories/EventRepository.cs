using Agendai.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agendai.Data.Database.Repositories
{
    public class EventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository()
        {
            _context = new AppDbContext();
        }

        // Obter todos os eventos
        public List<Event> GetAllEvents()
        {
            return _context.Events.ToList();
        }

        // Obter um evento por ID
        public Event GetEventById(ulong id)
        {
            return _context.Events.Find(id);
        }

        // Adicionar um novo evento
        public bool AddEvent(Event newEvent)
        {
            try
            {
                _context.Events.Add(newEvent);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Atualizar um evento existente
        public bool UpdateEvent(Event updatedEvent)
        {
            try
            {
                _context.Events.Update(updatedEvent);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Excluir um evento
        public bool DeleteEvent(ulong id)
        {
            try
            {
                var eventToDelete = _context.Events.Find(id);
                if (eventToDelete == null)
                    return false;
                    
                _context.Events.Remove(eventToDelete);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}