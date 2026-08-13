using System;
using System.Collections.Generic;
using NoteApp.Models;
using NoteApp.Repositories;

namespace NoteApp.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public List<Note> GetAllNotes()
        {
            return _noteRepository.GetAll();
        }

        public Note GetNoteById(int id)
        {
            return _noteRepository.GetById(id);
        }

        public void AddNote(Note note)
        {
            note.CreatedAt = DateTime.Now;
            _noteRepository.Add(note);
        }

        public void UpdateNote(Note note)
        {
            _noteRepository.Update(note);
        }

        public void DeleteNote(int id)
        {
            _noteRepository.Delete(id);
        }
    }
}