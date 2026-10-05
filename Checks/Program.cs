using System.IO;
using StudentGradesPortal.Models;
using StudentGradesPortal.Services;
using StudentGradesPortal.ViewModels;
using System.Text.Json;
internal static class Program
{
    static int count;
    static void Check(bool ok, string name) { if (!ok) throw new Exception(name); Console.WriteLine("PASS: " + name); count++; }
    [STAThread] static void Main()
    {
        var logic = new GradeAnalysisService();
        var a = new GradeRecord { StudentName = "Иван", GroupName = "ИС-21", Subject = "C#", WorkType = "Практическая", GradeValue = 5, IsFinal = true };
        var b = new GradeRecord { StudentName = "Анна", GradeValue = 2 };
        var records = new[] { a, b };
        Check(logic.Summarize(records) == new GradeSummary(2, 3.5, 1, 1, 1), "summary");
        Check(logic.Summarize(Array.Empty<GradeRecord>()).Average == 0, "empty summary");
        Check(logic.Matches(a, GradeStatusFilter.Excellent, "иван"), "case insensitive search");
        a.Comment = "Допущен к олимпиаде";
        Check(logic.Matches(a, GradeStatusFilter.All, "олимпиаде")
            && logic.Matches(a, GradeStatusFilter.All, "отлично"), "extended search fields");
        Check(logic.Matches(b, GradeStatusFilter.Failed, "") && !logic.Matches(a, GradeStatusFilter.Failed, ""), "failed filter");
        Check(logic.Matches(a, GradeStatusFilter.Passing, "") && !logic.Matches(b, GradeStatusFilter.Passing, ""), "passing filter");
        Check(logic.Matches(a, GradeStatusFilter.Final, "") && !logic.Matches(b, GradeStatusFilter.Final, ""), "final filter");
        Check(!logic.Matches(a, GradeStatusFilter.All, "нет"), "missing search");
        Check(!logic.IsValid(" ", "ИС", "C#", "Работа") && logic.IsValid("Иван", "ИС", "C#", "Работа"), "required fields");
        var path = Path.Combine(Path.GetTempPath(), "StudentGradesPortalChecks-" + Guid.NewGuid(), "grades.json");
        try
        {
            var storage = new JsonGradeStorageService(path);
            Check(storage.LoadRecords().Count > 0, "seed on first launch");
            storage.SaveRecords(records);
            var loaded = storage.LoadRecords();
            Check(loaded.Count == 2 && loaded[0].Id == a.Id && loaded[0].StudentName == "Иван", "JSON roundtrip");
            storage.SaveRecords(Array.Empty<GradeRecord>());
            Check(File.Exists(path + ".bak"), "backup before overwrite");
            Check(storage.LoadRecords().Count == 0, "empty journal persists");
            File.WriteAllText(path, "broken");
            bool rejected = false;
            try { storage.LoadRecords(); } catch (JsonException) { rejected = true; }
            Check(rejected && File.ReadAllText(path) == "broken", "invalid JSON preserved");
            storage.SaveRecords(records);
            var vm = new MainViewModel(storage, logic);
            vm.Editor.StudentName = "Новый студент";
            Check(vm.AddRecordCommand.CanExecute(null), "add enabled");
            vm.AddRecordCommand.Execute(null);
            Check(vm.TotalCount == 3 && storage.LoadRecords().Count == 3, "add and save");
            vm.Editor.GradeValue = 4;
            vm.SaveSelectedRecordCommand.Execute(null);
            Check(storage.LoadRecords().Last().GradeValue == 4, "edit and save");
            vm.SearchText = "Новый студент";
            Check(vm.RecordsView.Cast<GradeRecord>().Count() == 1, "view search integration");
            vm.DeleteRecordCommand.Execute(null);
            Check(vm.TotalCount == 2 && storage.LoadRecords().Count == 2, "delete and save");
        }
        finally { if (Directory.Exists(Path.GetDirectoryName(path))) Directory.Delete(Path.GetDirectoryName(path)!, true); }
        Console.WriteLine($"{count} checks passed");
    }
}

