using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using StudentGradesPortal.Data;
using StudentGradesPortal.Models;

namespace StudentGradesPortal.Services;

public sealed class JsonGradeStorageService : IGradeStorageService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _storagePath;

    public JsonGradeStorageService(string? storagePath = null)
    {
        _storagePath = storagePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudentGradesPortal", "grades.json");
    }

    public IReadOnlyList<GradeRecord> LoadRecords()
    {
        if (!File.Exists(_storagePath))
        {
            return GradeSeedData.CreateDefaultRecords();
        }

        var json = File.ReadAllText(_storagePath);
        return JsonSerializer.Deserialize<List<GradeRecord>>(json, _serializerOptions)
            ?? throw new JsonException("Файл журнала содержит null вместо списка оценок.");
    }

    public void SaveRecords(IEnumerable<GradeRecord> records)
    {
        var json = JsonSerializer.Serialize(records, _serializerOptions);
        var fullPath = Path.GetFullPath(_storagePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        var temporaryPath = fullPath + ".tmp";
        try
        {
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, fullPath, true);
        }
        finally { if (File.Exists(temporaryPath)) File.Delete(temporaryPath); }
    }
}
