using Microsoft.Data.Sqlite;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KatalogGierKomp
{
    public class DbManager
    {
        private readonly string ConnectionString = "";
        private readonly string DbPath = "";

        public static void Initialize(string DbPath)
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
                using (var connection = new SqliteConnection($"Data Source={DbPath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                    CREATE TABLE firmy (
                        id_firma INTEGER PRIMARY KEY,
                        nip TEXT NOT NULL UNIQUE
                            CHECK (length(nip) = 10 AND nip GLOB '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
                        nazwa_firmy TEXT NOT NULL,
                        nr_tel TEXT
                    );
                    ";
                    command.ExecuteNonQuery();
                    Console.WriteLine("Database created and initialized");
                }
            }
        }


    }
}