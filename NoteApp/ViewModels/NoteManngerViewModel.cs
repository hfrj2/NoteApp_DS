// ViewModels/NoteManageViewModel.cs
using NoteApp.Models;
using NoteApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NoteApp.ViewModels
{
    public class NoteManageViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Note> _notes;
        private Note _selectedNote;
        private Note _editingNote;
        private string _searchText;
        private bool _isEditing;
        private bool _showFavoritesOnly;

        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public Note SelectedNote
        {
            get => _selectedNote;
            set
            {
                SetProperty(ref _selectedNote, value);
                if (value != null)
                {
                    EditingNote = new Note
                    {
                        Id = value.Id,
                        UserId = value.UserId,
                        Title = value.Title,
                        Content = value.Content,
                        CreateTime = value.CreateTime,
                        UpdateTime = value.UpdateTime,
                        IsFavorite = value.IsFavorite,
                        Color = value.Color,
                        Category = value.Category
                    };
                }
                else
                {
                    EditingNote = null;
                }
                ((DelegateCommand)EditCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)ToggleFavoriteCommand).RaiseCanExecuteChanged();
            }
        }

        public Note EditingNote
        {
            get => _editingNote;
            set => SetProperty(ref _editingNote, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                LoadNotesAsync();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public bool ShowFavoritesOnly
        {
            get => _showFavoritesOnly;
            set
            {
                SetProperty(ref _showFavoritesOnly, value);
                LoadNotesAsync();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand RefreshCommand { get; }

        public NoteManageViewModel(IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            Notes = new ObservableCollection<Note>();

            AddCommand = new DelegateCommand(AddNote);
            EditCommand = new DelegateCommand(EditNote, CanEditNote);
            DeleteCommand = new DelegateCommand(DeleteNote, CanDeleteNote);
            SaveCommand = new DelegateCommand(SaveNote, CanSaveNote);
            CancelCommand = new DelegateCommand(CancelEdit);
            ToggleFavoriteCommand = new DelegateCommand(ToggleFavorite, CanToggleFavorite);
            RefreshCommand = new DelegateCommand(LoadNotesAsync);

            LoadNotesAsync();
        }

        private bool CanEditNote() => SelectedNote != null && !IsEditing;
        private bool CanDeleteNote() => SelectedNote != null && !IsEditing;
        private bool CanSaveNote() => IsEditing && EditingNote != null && !string.IsNullOrWhiteSpace(EditingNote.Title);
        private bool CanToggleFavorite() => SelectedNote != null && !IsEditing;

        private async void LoadNotesAsync()
        {
            try
            {
                var userId = SessionManager.CurrentUserId;
                var notes = await _dataService.SearchNotesAsync(userId, SearchText);

                if (ShowFavoritesOnly)
                {
                    notes = notes.Where(n => n.IsFavorite).ToList();
                }

                Notes.Clear();
                foreach (var note in notes)
                {
                    Notes.Add(note);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"加载便签失败：{ex.Message}");
            }
        }

        private void AddNote()
        {
            EditingNote = new Note
            {
                UserId = SessionManager.CurrentUserId,
                Title = "新便签",
                Content = "请输入内容...",
                CreateTime = DateTime.Now,
                Color = "#FFFFFF",
                Category = "默认"
            };
            IsEditing = true;
            SelectedNote = null;
        }

        private void EditNote()
        {
            if (SelectedNote == null) return;
            IsEditing = true;
        }

        private async void DeleteNote()
        {
            if (SelectedNote == null) return;

            if (_dialogService.ShowConfirm($"确定要删除便签 \"{SelectedNote.Title}\" 吗？"))
            {
                try
                {
                    var success = await _dataService.DeleteNoteAsync(SelectedNote.Id);
                    if (success)
                    {
                        Notes.Remove(SelectedNote);
                        SelectedNote = null;
                        _dialogService.ShowMessage("删除成功");
                    }
                    else
                    {
                        _dialogService.ShowError("删除失败");
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"删除失败：{ex.Message}");
                }
            }
        }

        private async void SaveNote()
        {
            if (EditingNote == null) return;

            if (string.IsNullOrWhiteSpace(EditingNote.Title))
            {
                _dialogService.ShowWarning("请输入便签标题");
                return;
            }

            try
            {
                bool success;
                if (EditingNote.Id == 0)
                {
                    success = await _dataService.AddNoteAsync(EditingNote);
                }
                else
                {
                    success = await _dataService.UpdateNoteAsync(EditingNote);
                }

                if (success)
                {
                    _dialogService.ShowMessage(EditingNote.Id == 0 ? "添加成功" : "更新成功");
                    IsEditing = false;
                    LoadNotesAsync();
                    if (EditingNote.Id > 0)
                    {
                        SelectedNote = Notes.FirstOrDefault(n => n.Id == EditingNote.Id);
                    }
                }
                else
                {
                    _dialogService.ShowError("保存失败");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"保存失败：{ex.Message}");
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            EditingNote = null;
            if (SelectedNote != null)
            {
                EditingNote = new Note
                {
                    Id = SelectedNote.Id,
                    UserId = SelectedNote.UserId,
                    Title = SelectedNote.Title,
                    Content = SelectedNote.Content,
                    CreateTime = SelectedNote.CreateTime,
                    UpdateTime = SelectedNote.UpdateTime,
                    IsFavorite = SelectedNote.IsFavorite,
                    Color = SelectedNote.Color,
                    Category = SelectedNote.Category
                };
            }
        }

        private async void ToggleFavorite()
        {
            if (SelectedNote == null) return;

            try
            {
                var success = await _dataService.ToggleFavoriteAsync(SelectedNote.Id);
                if (success)
                {
                    SelectedNote.IsFavorite = !SelectedNote.IsFavorite;
                    LoadNotesAsync();
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"操作失败：{ex.Message}");
            }
        }
    }
}