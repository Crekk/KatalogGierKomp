using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KatalogGierKomp
{
    public partial class GameFormWindow : Window
    {
        private readonly Game? editedGame;
        private byte[]? selectedImageBytes;

        public Game? GameResult { get; private set; }

        public GameFormWindow()
        {
            InitializeComponent();
        }

        public GameFormWindow(Game game)
        {
            InitializeComponent();

            editedGame = game;
            Title = "Edit Game";
            HeaderText.Text = "Edit game";
            SaveButton.Content = "Update";
            TitleBox.Text = game.Title;
            ScoreBox.Text = game.Score?.ToString() ?? "";
            ReviewBox.Text = game.Review;
            CompletionBox.SelectedIndex = game.Completion >= 0 && game.Completion <= 3 ? game.Completion : 0;
            ImageFileText.Text = game.Image is null ? "No PNG selected" : "Current PNG will be kept";
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

            using FileStream file = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
            selectedImageBytes = Utility.ImageToByteArray(file);
            ImageFileText.Text = Path.GetFileName(dialog.FileName);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string review = ReviewBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show(this, "Enter a title.", "Missing title", MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleBox.Focus();
                return;
            }

            int? score = null;
            string scoreText = ScoreBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(scoreText))
            {
                if (!int.TryParse(scoreText, out int parsedScore) || parsedScore < 0 || parsedScore > 10)
                {
                    MessageBox.Show(this, "Enter a score from 0 to 10, or leave it empty.", "Invalid score", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ScoreBox.Focus();
                    return;
                }

                score = parsedScore;
            }

            GameResult = new Game
            {
                Id = editedGame?.Id ?? 0,
                Title = title,
                Image = selectedImageBytes ?? editedGame?.Image,
                Score = score,
                Review = review,
                Completion = CompletionBox.SelectedIndex
            };

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
