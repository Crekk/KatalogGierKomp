using Microsoft.Data.Sqlite;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

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
            bool dbExists = File.Exists(DbPath);

            if (dbExists)
            {
                //later check if tables exist and are correct, if not, create or fix them, not just if the file exists
                Console.WriteLine("Database exists. Skipping initialization");
                return;
            }
            if (!dbExists)
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                    CREATE TABLE games (
                        id_game INTEGER PRIMARY KEY,
                        title TEXT,
                        image TEXT,
                        score INTEGER,
                        review TEXT,
                        completion INTEGER
                    );
                    ";
                    command.ExecuteNonQuery();
                    Console.WriteLine("Database created and initialized");
                }
            }
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
            FROM games;
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                games.Add(new Game
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Image = reader.IsDBNull(2) ? "" : reader.GetString(2),
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
            command.Parameters.AddWithValue("$image", game.Image);
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
            command.Parameters.AddWithValue("$image", game.Image);
            command.Parameters.AddWithValue("$score", game.Score);
            command.Parameters.AddWithValue("$review", game.Review);
            command.Parameters.AddWithValue("$completion", game.Completion);
            command.ExecuteNonQuery();
        }
    }
}
