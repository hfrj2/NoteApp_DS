using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using NoteApp.Models;
using NoteApp.Services;
using System.Windows;

namespace NoteApp.ViewModels
{
    public class NoteManageViewModel : BindableBase
    {
        private readonly INoteService _noteService;

        private ObservableCollection<Note> _notes;
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private Note _selectedNote;
        public Note SelectedNote
        {
            get => _selectedNote;
            set
            {
                if (SetProperty(ref _selectedNote, value) && value != null)
                {
                    // 填充编辑表单
                    EditId = value.Id;
                    EditTitle = value.Title;
                    EditContent = value.Content;
                    EditCreatedAt = value.CreatedAt;
                }
            }
        }

        private int _editId;
        public int EditId
        {
            get => _editId;
            set => SetProperty(ref _editId, value);
        }

        private string _editTitle;
        public string EditTitle
        {
            get => _editTitle;
            set => SetProperty(ref _editTitle, value);
        }

        private string _editContent;
        public string EditContent
        {
            get => _editContent;
            set => SetProperty(ref _editContent, value);
        }

        private DateTime _editCreatedAt;
        public DateTime EditCreatedAt
        {
            get => _editCreatedAt;
            set => SetProperty(ref _editCreatedAt, value);
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand RefreshCommand { get; }

        public NoteManageViewModel(INoteService noteService)
        {
            _noteService = noteService;
            AddCommand = new DelegateCommand(Add);
            SaveCommand = new DelegateCommand(Save, CanSave);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            RefreshCommand = new DelegateCommand(Refresh);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(EditTitle) || e.PropertyName == nameof(EditContent))
                    SaveCommand.RaiseCanExecuteChanged();
                if (e.PropertyName == nameof(SelectedNote))
                    DeleteCommand.RaiseCanExecuteChanged();
            };

            Refresh();
        }

        private void Add()
        {
            SelectedNote = null;
            EditId = 0;
            EditTitle = string.Empty;
            EditContent = string.Empty;
            EditCreatedAt = DateTime.Now;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(EditTitle);
        }

        private void Save()
        {
            if (EditId == 0)
            {
                // 新增
                var note = new Note
                {
                    Title = EditTitle.Trim(),
                    Content = EditContent,
                    CreatedAt = EditCreatedAt
                };
                _noteService.AddNote(note);
            }
            else
            {
                // 编辑
                var note = _noteService.GetNoteById(EditId);
                if (note != null)
                {
                    note.Title = EditTitle.Trim();
                    note.Content = EditContent;
                    // 创建时间保持不变
                    _noteService.UpdateNote(note);
                }
            }
            Refresh();
        }

        private bool CanDelete()
        {
            return SelectedNote != null;
        }

        private void Delete()
        {
            if (SelectedNote == null) return;
            var result = MessageBox.Show($"确定要删除便签“{SelectedNote.Title}”吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _noteService.DeleteNote(SelectedNote.Id);
                Refresh();
            }
        }

        private void Refresh()
        {
            var list = _noteService.GetAllNotes();
            Notes = new ObservableCollection<Note>(list);
            if (Notes.Any())
            {
                SelectedNote = Notes[0];
            }
            else
            {
                SelectedNote = null;
                Add();
            }
        }
    }
}