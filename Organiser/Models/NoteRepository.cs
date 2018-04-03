using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Organiser.Controllers;

namespace Organiser.Models
{
    public class NoteRepository : INoteRepository
    {
        public AppDbContext _appDbContext;

        public NoteRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public bool HasNewMessages(int location)
        {
            NewMessagesMonitor monitor = _appDbContext.NewMessagesMonitor.First();
            switch (location)
            { 
                case 1:
                return monitor.Folirane;
                case 2:
                    return monitor.ManualWork;
                case 3:
                    return monitor.InkChet;
                case 4:
                    return monitor.Falcing;
                case 5:
                    return monitor.Kovertirane;
                case 6:
                    return monitor.Sklad;
                case 7:
                    return monitor.Drivers;
                default:
            return false;
            }
        }

        public bool MonitorIsCreated()
        {
            return _appDbContext.NewMessagesMonitor.Any();
        }

        public void CreateMonitor()
        {
            _appDbContext.Update(new NewMessagesMonitor());
            _appDbContext.SaveChanges();
        }

        public void UpdateMonitor(int location, bool hasNewMessages)
        {
            NewMessagesMonitor monitor = _appDbContext.NewMessagesMonitor.FirstOrDefault();
            switch (location)
            {
                case 1:
                    monitor.Folirane = hasNewMessages;
                    break;
                case 2:
                     monitor.ManualWork = hasNewMessages;
                    break;
                case 3:
                    monitor.InkChet = hasNewMessages;
                    break;
                case 4:
                    monitor.Falcing = hasNewMessages;
                    break;
                case 5:
                    monitor.Kovertirane = hasNewMessages;
                    break;
                case 6:
                     monitor.Sklad = hasNewMessages;
                    break;
                case 7:
                     monitor.Drivers = hasNewMessages;
                    break;
            }
            _appDbContext.Update(monitor);
            _appDbContext.SaveChanges();

        }

        public IQueryable<Note> GetNotesForLocation(int location) =>
                    _appDbContext.Notes.Where(n => n.Location == location)
            .OrderByDescending(n => n.CreatedAt);

        //public IQueryable<Note> GetNotesForLocation(int location) =>
        //         _appDbContext.Notes.Where(n => n.Location == location)
        // .OrderByDescending(n => n.CreatedAt);

        public Note GetOldestNote()
        {
            return _appDbContext.Notes.OrderBy(n => n.CreatedAt).FirstOrDefault();
        }

        public void EraseNotesOlderThanDate(DateTime date)
        {
            List<Note> notes = _appDbContext.Notes.Where(n => n.CreatedAt < date).ToList();
            _appDbContext.RemoveRange(notes);
            _appDbContext.SaveChanges();
        }
        
    }
}
