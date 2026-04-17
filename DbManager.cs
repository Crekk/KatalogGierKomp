using Microsoft.Data.Sqlite;
namespace KatalogGierKomp
{
    public class DbManager
    {
        private readonly string DbPath;
        private readonly string ConnectionString;

        public DbManager()
        {
            DbPath = "games.db";
            ConnectionString = $"Data Source={DbPath}";
        }

        public DbManager(string dbPath)
        {
            DbPath = dbPath;
            ConnectionString = $"Data Source={DbPath}";
        }

        public void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS games (
                id_game INTEGER PRIMARY KEY,
                title TEXT,
                image BLOB,
                score INTEGER,
                review TEXT,
                completion INTEGER
            );
            ";
            command.ExecuteNonQuery();
        }

        public List<Game> LoadGames()
        {
            List<Game> games = new List<Game>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
            @"
            SELECT id_game, title, image, score, review, completion
            FROM games
            ORDER BY id_game DESC;
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                games.Add(new Game
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Image = reader.IsDBNull(2) ? null : reader.GetFieldValue<byte[]>(2),
                    Score = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    Review = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Completion = reader.IsDBNull(5) ? 0 : reader.GetInt32(5)
                });
            }

            return games;
        }

        public void AddGame(Game game)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
            @"
            INSERT INTO games (title, image, score, review, completion)
            VALUES ($title, $image, $score, $review, $completion);
            ";
            command.Parameters.AddWithValue("$title", game.Title);
            command.Parameters.AddWithValue("$image", game.Image is null ? DBNull.Value : game.Image);
            command.Parameters.AddWithValue("$score", game.Score);
            command.Parameters.AddWithValue("$review", game.Review);
            command.Parameters.AddWithValue("$completion", game.Completion);
            command.ExecuteNonQuery();
        }

        public void EditGame(Game game)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
            @"
            UPDATE games
            SET title = $title,
                image = $image,
                score = $score,
                review = $review,
                completion = $completion
            WHERE id_game = $id;
            ";
            command.Parameters.AddWithValue("$id", game.Id);
            command.Parameters.AddWithValue("$title", game.Title);
            command.Parameters.AddWithValue("$image", game.Image is null ? DBNull.Value : game.Image);
            command.Parameters.AddWithValue("$score", game.Score);
            command.Parameters.AddWithValue("$review", game.Review);
            command.Parameters.AddWithValue("$completion", game.Completion);
            command.ExecuteNonQuery();
        }
    }
}
