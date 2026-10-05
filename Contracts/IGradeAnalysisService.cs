using StudentGradesPortal.Models;
namespace StudentGradesPortal.Services;
public interface IGradeAnalysisService
{
    bool Matches(GradeRecord record, GradeStatusFilter filter, string search);
    GradeSummary Summarize(IEnumerable<GradeRecord> records);
    bool IsValid(string student, string group, string subject, string workType);
}
