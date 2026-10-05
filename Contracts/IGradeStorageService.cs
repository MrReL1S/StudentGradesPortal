using System.Collections.Generic;
using StudentGradesPortal.Models;

namespace StudentGradesPortal.Services;

public interface IGradeStorageService
{
    IReadOnlyList<GradeRecord> LoadRecords();

    void SaveRecords(IEnumerable<GradeRecord> records);
}
