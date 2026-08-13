using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
using NoteApp.Models;

namespace NoteApp.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public NoteRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Note GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Notes.Find(id);
        }

        public List<Note> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Notes.ToList();
        }

        public void Add(Note note)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Notes.Add(note);
            context.SaveChanges();
        }

        public void Update(Note note)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Notes.Update(note);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var note = context.Notes.Find(id);
            if (note != null)
            {
                context.Notes.Remove(note);
                context.SaveChanges();
            }
        }
    }
}