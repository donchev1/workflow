using System;
using System.Linq;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public interface INoteRepository
    {
        void UpdateMonitor(int location, bool hasNewMessages);
        void CreateMonitor();
        bool MonitorIsCreated();
        bool HasNewMessages(int location);
        IQueryable<Note> GetNotesForLocation(int location);
        Note GetOldestNote();
        void EraseNotesOlderThanDate(DateTime date);
    }
}
