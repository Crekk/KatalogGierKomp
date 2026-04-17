using System.Windows;
using System.Windows.Controls;

namespace KatalogGierKomp
{
    public partial class MainWindow : Window
    {
        private readonly DbManager dbManager = new();

        public MainWindow()
        {
            InitializeComponent();
            dbManager.Initialize();
            ShowGames();
        }

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            GameFormWindow window = new GameFormWindow();
            window.Owner = this;

            if (window.ShowDialog() == true && window.GameResult != null)
            {
                dbManager.AddGame(window.GameResult);
                ShowGames();
            }
        }

        private void EditGameButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Game game = (Game)button.CommandParameter;

            GameFormWindow window = new GameFormWindow(game);
            window.Owner = this;

            if (window.ShowDialog() == true && window.GameResult != null)
            {
                dbManager.EditGame(window.GameResult);
                ShowGames();
            }
        }

        private void ViewGameButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Game game = (Game)button.CommandParameter;

            GameDetailsWindow window = new GameDetailsWindow(game);
            window.Owner = this;
            window.ShowDialog();
        }

        private void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Game game = (Game)button.CommandParameter;

            MessageBoxResult answer = MessageBox.Show(
                $"Delete \"{game.Title}\"?",
                "Delete game",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (answer == MessageBoxResult.Yes)
            {
                dbManager.DeleteGame(game.Id);
                ShowGames();
            }
        }

        private void FilterOrSortChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GamesList != null)
            {
                ShowGames();
            }
        }

        private void ShowGames()
        {
            List<Game> games = dbManager.LoadGames();

            int status = FilterBox.SelectedIndex - 1;
            if (status >= 0)
            {
                games = games.Where(game => game.Completion == status).ToList();
            }

            switch (SortBox.SelectedIndex)
            {
                case 1:
                    games = games.OrderBy(game => game.Title).ToList();
                    break;
                case 2:
                    games = games.OrderByDescending(game => game.Title).ToList();
                    break;
                case 3:
                    games = games.OrderBy(game => game.Score == null)
                                 .ThenByDescending(game => game.Score)
                                 .ToList();
                    break;
                case 4:
                    games = games.OrderBy(game => game.Score == null)
                                 .ThenBy(game => game.Score)
                                 .ToList();
                    break;
                case 5:
                    games = games.OrderBy(game => game.Completion).ToList();
                    break;
            }

            GamesList.ItemsSource = games;
            GameCountText.Text = $"{games.Count} {(games.Count == 1 ? "game" : "games")}";
            EmptyMessage.Visibility = games.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
