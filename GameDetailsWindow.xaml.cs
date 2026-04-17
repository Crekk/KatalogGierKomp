using System.Windows;

namespace KatalogGierKomp
{
    public partial class GameDetailsWindow : Window
    {
        public GameDetailsWindow(Game game)
        {
            InitializeComponent();
            DataContext = game;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
