using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KatalogGierKomp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DbManager dbManager = new();
        private List<Game> games = new();
        public MainWindow()
        {
            InitializeComponent();

            dbManager.Initialize();
            games = dbManager.LoadGames();
            GamesList.ItemsSource = games;
            GameCountText.Text = $"{games.Count} {(games.Count == 1 ? "gra" : "gier")}";
            EmptyMessage.Visibility = games.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
