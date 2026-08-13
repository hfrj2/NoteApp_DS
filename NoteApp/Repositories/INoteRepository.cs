using System.Collections.Generic;
using NoteApp.Models;

namespace NoteApp.Repositories
{
    public interface INoteRepository
    {
        Note GetById(int id);
        List<Note> GetAll();
        void Add(Note note);
        void Update(Note note);
        void Delete(int id);
    }
}