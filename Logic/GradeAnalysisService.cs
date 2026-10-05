using StudentGradesPortal.Models;
namespace StudentGradesPortal.Services;
/// <summary>Pure journal rules, independent of WPF and storage.</summary>
public sealed class GradeAnalysisService : IGradeAnalysisService
{
    public bool Matches(GradeRecord record, GradeStatusFilter filter, string search)
    {
        bool matches = filter switch
        {
            GradeStatusFilter.Excellent => record.GradeValue == 5,
            GradeStatusFilter.Passing => record.IsPassing,
            GradeStatusFilter.Failed => !record.IsPassing,
            GradeStatusFilter.Final => record.IsFinal,
            _ => true
        };
        if (!matches) return false;
        var normalizedSearch = NormalizeSearch(search);
        if (normalizedSearch.Length == 0) return true;
        return new[]
            {
                record.StudentName,
                record.GroupName,
                record.Subject,
                record.WorkType,
                record.Comment,
                record.GradeLabel
            }
            .Any(value => value.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
    }
    public GradeSummary Summarize(IEnumerable<GradeRecord> records)
    {
        var list = records.ToList();
        return new(list.Count, list.Count == 0 ? 0 : Math.Round(list.Average(r => r.GradeValue), 2),
            list.Count(r => r.GradeValue == 5), list.Count(r => !r.IsPassing), list.Count(r => r.IsFinal));
    }
    public bool IsValid(string student, string group, string subject, string workType) =>
        new[] { student, group, subject, workType }.All(s => !string.IsNullOrWhiteSpace(s));

    private static string NormalizeSearch(string search) => search.Trim();
}
