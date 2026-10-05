using System;
using System.Text.Json.Serialization;
using StudentGradesPortal.Common;

namespace StudentGradesPortal.Models;

public sealed class GradeRecord : ObservableObject
{
    private DateTime _assessmentDate = DateTime.Today;
    private string _comment = string.Empty;
    private int _gradeValue = 5;
    private string _groupName = string.Empty;
    private bool _isFinal;
    private string _studentName = string.Empty;
    private string _subject = string.Empty;
    private string _workType = string.Empty;

    public Guid Id { get; set; } = Guid.NewGuid();

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
        set
        {
            var normalized = Math.Clamp(value, 2, 5);
            if (SetProperty(ref _gradeValue, normalized))
            {
                OnPropertyChanged(nameof(IsPassing));
                OnPropertyChanged(nameof(GradeLabel));
            }
        }
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

    [JsonIgnore]
    public string GradeLabel => GradeValue switch
    {
        5 => "Отлично",
        4 => "Хорошо",
        3 => "Удовлетворительно",
        _ => "Неудовлетворительно"
    };

    [JsonIgnore]
    public bool IsPassing => GradeValue >= 3;
}
