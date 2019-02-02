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
                return monitor.Folirung;
                case 2:
                    return monitor.Handarbeit;
                case 3:
                    return monitor.Inkchet;
                case 4:
                    return monitor.Falcen;
                case 5:
                    return monitor.Covertirung;
                case 6:
                    return monitor.Lager;
                case 7:
                    return monitor.Fahrer;
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
                    monitor.Folirung = hasNewMessages;
                    break;
                case 2:
                     monitor.Handarbeit = hasNewMessages;
                    break;
                case 3:
                    monitor.Inkchet = hasNewMessages;
                    break;
                case 4:
                    monitor.Falcen = hasNewMessages;
                    break;
                case 5:
                    monitor.Covertirung = hasNewMessages;
                    break;
                case 6:
                     monitor.Lager = hasNewMessages;
                    break;
                case 7:
                     monitor.Fahrer = hasNewMessages;
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
