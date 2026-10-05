using System.Windows;
using StudentGradesPortal.Services;
using StudentGradesPortal.ViewModels;

namespace StudentGradesPortal;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        try
        {
            DataContext = new MainViewModel(new JsonGradeStorageService(), new GradeAnalysisService());
        }
        catch (System.Exception ex) when (ex is System.IO.IOException or System.UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            MessageBox.Show("Не удалось загрузить журнал. Исходный файл не изменён.\n" + ex.Message,
                "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown(1);
        }
    }
}
