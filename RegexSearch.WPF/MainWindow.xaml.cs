using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RegexSearch.Core.Models;
using RegexSearch.Core.Services;

namespace RegexSearch.WPF
{
    public partial class MainWindow : Window
    {
        private readonly RecognitionService _service = new RecognitionService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Show(List<MatchResult> results, string mode)
        {
            ResultsGrid.ItemsSource = results;
            SummaryText.Text = $"Режим: {mode}. Найдено совпадений: {results.Count}";
            StatusText.Text = "Поиск выполнен.";
        }

        private void RegexButton_Click(object sender, RoutedEventArgs e)
        {
            Show(_service.FindWithRegex(InputText.Text), "Regex");
        }

        private void AutomatonButton_Click(object sender, RoutedEventArgs e)
        {
            Show(_service.FindWithAutomaton(InputText.Text), "Автомат");
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            var a = _service.FindWithRegex(InputText.Text);
            var b = _service.FindWithAutomaton(InputText.Text);

            bool same = a.Count == b.Count &&
                a.Zip(b, (x, y) => x.Value == y.Value && x.Position == y.Position).All(r => r);

            ResultsGrid.ItemsSource = a;
            SummaryText.Text = same
                ? $"Результаты совпадают. Найдено: {a.Count}"
                : $"Результаты РАЗНЫЕ. Regex: {a.Count}, автомат: {b.Count}";
            StatusText.Text = "Сравнение выполнено.";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputText.Clear();
            ResultsGrid.ItemsSource = null;
            SummaryText.Text = "Результаты поиска появятся здесь.";
            StatusText.Text = "Программа готова к работе.";
        }
    }
}
