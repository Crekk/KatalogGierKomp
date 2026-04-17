using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KatalogGierKomp
{
    public partial class MainWindow : Window
    {
        private readonly DbManager dbManager = new();
        private List<Game> games = new();
        private byte[]? selectedImageBytes;

        public MainWindow()
        {
            InitializeComponent();

            dbManager.Initialize();
            RefreshGames();
        }

        private void ShowAddFormButton_Click(object sender, RoutedEventArgs e)
        {
            AddGamePanel.Visibility = Visibility.Visible;
            TitleBox.Focus();
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Choose a PNG cover image",
                Filter = "PNG images (*.png)|*.png",
                Multiselect = false
            };

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            selectedImageBytes = File.ReadAllBytes(dialog.FileName);
            ImageFileText.Text = Path.GetFileName(dialog.FileName);
        }

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string review = ReviewBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show(this, "Enter a title.", "Missing title", MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleBox.Focus();
                return;
            }

            if (!int.TryParse(ScoreBox.Text.Trim(), out int score) || score < 0 || score > 10)
            {
                MessageBox.Show(this, "Enter a score from 0 to 10.", "Invalid score", MessageBoxButton.OK, MessageBoxImage.Warning);
                ScoreBox.Focus();
                return;
            }

            dbManager.AddGame(new Game
            {
                Title = title,
                Image = selectedImageBytes,
                Score = score,
                Review = review,
                Completion = GetSelectedCompletionStatus()
            });

            ResetAddForm();
            RefreshGames();
        }

        private void CancelAddButton_Click(object sender, RoutedEventArgs e)
        {
            ResetAddForm();
        }

        private void RefreshGames()
        {
            games = dbManager.LoadGames();
            GamesList.ItemsSource = games;
            GameCountText.Text = $"{games.Count} {(games.Count == 1 ? "game" : "games")}";
            EmptyMessage.Visibility = games.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private int GetSelectedCompletionStatus()
        {
            if (CompletionBox.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Tag?.ToString(), out int completion))
            {
                return completion;
            }

            return 0;
        }

        private void ResetAddForm()
        {
            TitleBox.Clear();
            ScoreBox.Clear();
            ReviewBox.Clear();
            CompletionBox.SelectedIndex = 0;
            selectedImageBytes = null;
            ImageFileText.Text = "No PNG selected";
            AddGamePanel.Visibility = Visibility.Collapsed;
        }
    }
}
