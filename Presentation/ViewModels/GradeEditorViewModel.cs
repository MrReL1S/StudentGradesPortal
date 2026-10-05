using System;
using StudentGradesPortal.Common;
using StudentGradesPortal.Models;

namespace StudentGradesPortal.ViewModels;

public sealed class GradeEditorViewModel : ObservableObject
{
    private DateTime _assessmentDate = DateTime.Today;
    private string _comment = string.Empty;
    private int _gradeValue = 5;
    private string _groupName = "ИС-21";
    private bool _isFinal;
    private string _studentName = string.Empty;
    private string _subject = "Программирование";
    private string _workType = "Практическая работа";

    public DateTime AssessmentDate
    {
        get => _assessmentDate;
        set => SetProperty(ref _assessmentDate, value);
    }

    public string Comment
    {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }

    public int GradeValue
    {
        get => _gradeValue;
        set => SetProperty(ref _gradeValue, Math.Clamp(value, 2, 5));
    }

    public string GroupName
    {
        get => _groupName;
        set => SetProperty(ref _groupName, value);
    }

    public bool IsFinal
    {
        get => _isFinal;
        set => SetProperty(ref _isFinal, value);
    }

    public string StudentName
    {
        get => _studentName;
        set => SetProperty(ref _studentName, value);
    }

    public string Subject
    {
        get => _subject;
        set => SetProperty(ref _subject, value);
    }

    public string WorkType
    {
        get => _workType;
        set => SetProperty(ref _workType, value);
    }

    public void Load(GradeRecord? record)
    {
        StudentName = record?.StudentName ?? string.Empty;
        GroupName = record?.GroupName ?? "ИС-21";
        Subject = record?.Subject ?? "Программирование";
        WorkType = record?.WorkType ?? "Практическая работа";
        GradeValue = record?.GradeValue ?? 5;
        AssessmentDate = record?.AssessmentDate ?? DateTime.Today;
        IsFinal = record?.IsFinal ?? false;
        Comment = record?.Comment ?? string.Empty;
    }
}
