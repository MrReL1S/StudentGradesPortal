using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using StudentGradesPortal.Commands;
using StudentGradesPortal.Common;
using StudentGradesPortal.Models;
using StudentGradesPortal.Services;

namespace StudentGradesPortal.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly IGradeStorageService _storageService;
    private readonly IGradeAnalysisService _analysis;
    private bool _isEditing;
    private GradeSummary _summary = new(0, 0, 0, 0, 0);
    private readonly ObservableCollection<GradeRecord> _records = new();
    private GradeStatusFilter _selectedFilter = GradeStatusFilter.All;
    private GradeRecord? _selectedRecord;
    private string _statusMessage = "Журнал готов к работе";
    private string _searchText = string.Empty;

    public MainViewModel(IGradeStorageService storageService, IGradeAnalysisService analysis)
    {
        _storageService = storageService;
        _analysis = analysis;

        foreach (var record in _storageService.LoadRecords())
        {
            TrackRecord(record);
            _records.Add(record);
        }

        RecordsView = CollectionViewSource.GetDefaultView(_records);
        RecordsView.Filter = FilterRecord;

        Editor = new GradeEditorViewModel();
        GradeOptions = new[] { 5, 4, 3, 2 };
        GroupOptions = new[] { "ИС-21", "ПИ-22", "КС-23", "ДО-24" };
        SubjectOptions = new[] { "Программирование", "Базы данных", "Информационные системы", "Проектирование ПО" };
        WorkTypeOptions = new[] { "Практическая работа", "Лабораторная работа", "Контрольная работа", "Итоговая оценка" };

        AddRecordCommand = new RelayCommand(_ => AddRecord(), _ => CanSaveEditor());
        SaveSelectedRecordCommand = new RelayCommand(_ => SaveSelectedRecord(), _ => HasSelectedRecord && CanSaveEditor());
        DeleteRecordCommand = new RelayCommand(DeleteRecord, parameter => parameter is GradeRecord || HasSelectedRecord);
        ClearFailedCommand = new RelayCommand(_ => ClearFailed(), _ => _records.Any(record => !record.IsPassing));
        SetFilterCommand = new RelayCommand(SetFilter);

        SelectedRecord = _records.FirstOrDefault();
        RefreshSummary();
    }

    public ICommand AddRecordCommand { get; }

    public double AverageGrade => _summary.Average;

    public ICommand ClearFailedCommand { get; }

    public int ExcellentCount => _summary.Excellent;

    public ICommand DeleteRecordCommand { get; }

    public GradeEditorViewModel Editor { get; }

    public int FailedCount => _summary.Failed;

    public int FinalCount => _summary.Final;

    public int[] GradeOptions { get; }

    public string[] GroupOptions { get; }

    public bool HasSelectedRecord => SelectedRecord is not null;

    public ICollectionView RecordsView { get; }

    public ICommand SaveSelectedRecordCommand { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                RecordsView.Refresh();
                RefreshSummary();
            }
        }
    }

    public GradeStatusFilter SelectedFilter
    {
        get => _selectedFilter;
        private set
        {
            if (SetProperty(ref _selectedFilter, value))
            {
                OnPropertyChanged(nameof(CurrentFilterTitle));
                RecordsView.Refresh();
                RefreshSummary();
            }
        }
    }

    public GradeRecord? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            if (SetProperty(ref _selectedRecord, value))
            {
                Editor.Load(value);
                OnPropertyChanged(nameof(HasSelectedRecord));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public ICommand SetFilterCommand { get; }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string[] SubjectOptions { get; }

    public int TotalCount => _summary.Total;

    public string[] WorkTypeOptions { get; }

    public string CurrentFilterTitle => SelectedFilter switch
    {
        GradeStatusFilter.Excellent => "отличные оценки",
        GradeStatusFilter.Passing => "без задолженностей",
        GradeStatusFilter.Failed => "требуют пересдачи",
        GradeStatusFilter.Final => "итоговые оценки",
        _ => "все записи"
    };

    private void AddRecord()
    {
        if (!CanSaveEditor())
        {
            StatusMessage = "Укажите студента и дисциплину";
            return;
        }

        var record = new GradeRecord
        {
            StudentName = Editor.StudentName.Trim(),
            GroupName = Editor.GroupName.Trim(),
            Subject = Editor.Subject.Trim(),
            WorkType = Editor.WorkType.Trim(),
            GradeValue = Editor.GradeValue,
            AssessmentDate = Editor.AssessmentDate,
            IsFinal = Editor.IsFinal,
            Comment = Editor.Comment.Trim()
        };

        TrackRecord(record);
        _records.Add(record);
        SelectedRecord = record;
        SaveAndRefresh($"Добавлена оценка для студента: {record.StudentName}");
    }

    private bool CanSaveEditor()
    {
        return _analysis.IsValid(Editor.StudentName, Editor.GroupName, Editor.Subject, Editor.WorkType);
    }

    private void ClearFailed()
    {
        var failedRecords = _records.Where(record => !record.IsPassing).ToList();
        foreach (var record in failedRecords)
        {
            record.PropertyChanged -= OnRecordChanged;
            _records.Remove(record);
        }

        if (SelectedRecord is not null && failedRecords.Contains(SelectedRecord))
        {
            SelectedRecord = _records.FirstOrDefault();
        }

        SaveAndRefresh("Записи с неудовлетворительными оценками удалены");
    }

    private void DeleteRecord(object? parameter)
    {
        var recordToDelete = parameter as GradeRecord ?? SelectedRecord;
        if (recordToDelete is null)
        {
            return;
        }

        var deletedStudent = recordToDelete.StudentName;
        recordToDelete.PropertyChanged -= OnRecordChanged;
        _records.Remove(recordToDelete);

        if (ReferenceEquals(SelectedRecord, recordToDelete))
        {
            SelectedRecord = _records.FirstOrDefault();
        }

        SaveAndRefresh($"Удалена запись: {deletedStudent}");
    }

    private bool FilterRecord(object item) => item is GradeRecord record
        && _analysis.Matches(record, SelectedFilter, SearchText);

    private void OnRecordChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isEditing) return;
        if (e.PropertyName is nameof(GradeRecord.GradeValue)
            or nameof(GradeRecord.StudentName)
            or nameof(GradeRecord.GroupName)
            or nameof(GradeRecord.Subject)
            or nameof(GradeRecord.WorkType)
            or nameof(GradeRecord.AssessmentDate)
            or nameof(GradeRecord.IsFinal)
            or nameof(GradeRecord.Comment))
        {
            SaveAndRefresh("Изменения сохранены");
        }
    }

    private void RefreshSummary()
    {
        _summary = _analysis.Summarize(_records);
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(AverageGrade));
        OnPropertyChanged(nameof(ExcellentCount));
        OnPropertyChanged(nameof(FailedCount));
        OnPropertyChanged(nameof(FinalCount));
        CommandManager.InvalidateRequerySuggested();
    }

    private void SaveAndRefresh(string statusMessage)
    {
        try { _storageService.SaveRecords(_records); }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
        {
            RecordsView.Refresh();
            RefreshSummary();
            StatusMessage = "Ошибка сохранения: " + ex.Message;
            return;
        }
        RecordsView.Refresh();
        RefreshSummary();
        StatusMessage = statusMessage;
    }

    private void SaveSelectedRecord()
    {
        if (SelectedRecord is null)
        {
            return;
        }

        if (!CanSaveEditor())
        {
            StatusMessage = "Проверьте обязательные поля";
            return;
        }

        _isEditing = true;
        try
        {
        SelectedRecord.StudentName = Editor.StudentName.Trim();
        SelectedRecord.GroupName = Editor.GroupName.Trim();
        SelectedRecord.Subject = Editor.Subject.Trim();
        SelectedRecord.WorkType = Editor.WorkType.Trim();
        SelectedRecord.GradeValue = Editor.GradeValue;
        SelectedRecord.AssessmentDate = Editor.AssessmentDate;
        SelectedRecord.IsFinal = Editor.IsFinal;
        SelectedRecord.Comment = Editor.Comment.Trim();
        }
        finally { _isEditing = false; }

        SaveAndRefresh($"Сохранена запись: {SelectedRecord.StudentName}");
    }

    private void SetFilter(object? parameter)
    {
        if (parameter is GradeStatusFilter filter)
        {
            SelectedFilter = filter;
            StatusMessage = $"Показаны: {CurrentFilterTitle}";
        }
    }

    private void TrackRecord(GradeRecord record)
    {
        record.PropertyChanged += OnRecordChanged;
    }
}
