using System;
using System.Collections.Generic;
using StudentGradesPortal.Models;

namespace StudentGradesPortal.Data;

public static class GradeSeedData
{
    public static IReadOnlyList<GradeRecord> CreateDefaultRecords()
    {
        return new[]
        {
            new GradeRecord
            {
                StudentName = "Иванова Мария",
                GroupName = "ИС-21",
                Subject = "Программирование",
                WorkType = "Практическая работа",
                GradeValue = 5,
                AssessmentDate = DateTime.Today,
                IsFinal = false,
                Comment = "Работа выполнена полностью"
            },
            new GradeRecord
            {
                StudentName = "Петров Артем",
                GroupName = "ИС-21",
                Subject = "Базы данных",
                WorkType = "Контрольная работа",
                GradeValue = 4,
                AssessmentDate = DateTime.Today.AddDays(-1),
                IsFinal = false,
                Comment = "Есть мелкие недочеты в запросах"
            },
            new GradeRecord
            {
                StudentName = "Смирнова Анна",
                GroupName = "ПИ-22",
                Subject = "Информационные системы",
                WorkType = "Итоговая оценка",
                GradeValue = 5,
                AssessmentDate = DateTime.Today.AddDays(-2),
                IsFinal = true,
                Comment = "Высокая активность в течение модуля"
            },
            new GradeRecord
            {
                StudentName = "Козлов Никита",
                GroupName = "ПИ-22",
                Subject = "Программирование",
                WorkType = "Лабораторная работа",
                GradeValue = 2,
                AssessmentDate = DateTime.Today.AddDays(-3),
                IsFinal = false,
                Comment = "Нужна пересдача"
            }
        };
    }
}
