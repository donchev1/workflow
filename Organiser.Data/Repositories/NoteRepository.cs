using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Organiser.Data.Context;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class NoteRepository : Repository<AppDbContext, Note>
    {
        public  AppDbContext _context;
        public NoteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public bool HasNewMessages(int location)
        {
            var monitor = _context.NewMessagesMonitor.First();
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
            return _context.NewMessagesMonitor.Any();
        }

        public void CreateMonitor()
        {
            _context.Update(new NewMessagesMonitor());
            _context.SaveChanges();
        }

        public void UpdateMonitor(int location, bool hasNewMessages)
        {
            NewMessagesMonitor monitor = _context.NewMessagesMonitor.FirstOrDefault();
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
            _context.Update(monitor);
            _context.SaveChanges();

        }

        public IQueryable<Note> GetNotesForLocation(int location) =>
                    _context.Notes.Where(n => n.Location == location)
            .OrderByDescending(n => n.CreatedAt);

        //public IQueryable<Note> GetNotesForLocation(int location) =>
        //         _appDbContext.Notes.Where(n => n.Location == location)
        // .OrderByDescending(n => n.CreatedAt);

        public Note GetOldestNote()
        {
            return _context.Notes.OrderBy(n => n.CreatedAt).FirstOrDefault();
        }

        public void EraseNotesOlderThanDate(DateTime date)
        {
            List<Note> notes = _context.Notes.Where(n => n.CreatedAt < date).ToList();
            _context.RemoveRange(notes);
            _context.SaveChanges();
        }
    }
}
