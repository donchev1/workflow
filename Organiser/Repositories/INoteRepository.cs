using Organiser.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
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
