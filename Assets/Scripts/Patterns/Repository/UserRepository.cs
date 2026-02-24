using UnityEngine;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite; 
using System.IO;

public class UserRepository
{
    private string dbPath;

    public UserRepository()
    {

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            dbPath = "URI=file:" + Application.dataPath + "/game_data.db";
        }
        else
        {
            string folder = System.IO.Path.GetDirectoryName(Application.dataPath);
            dbPath = "URI=file:" + System.IO.Path.Combine(folder, "game_data.db");
        }

        CreateDatabase();
    }

    private void CreateDatabase()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        Wins INTEGER DEFAULT 0,
                        Losses INTEGER DEFAULT 0
                    );";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    public bool Register(string username, string password)
    {
        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Users (Username, Password, Wins, Losses) VALUES (@u, @p, 0, 0)";
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Kayýt Hatasý (Muhtemelen isim dolu): " + e.Message);
            return false;
        }
    }

    public User Login(string username, string password)
    {
        User foundUser = null;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Username = @u AND Password = @p";
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        foundUser = ParseUser(reader);
                    }
                }
            }
        }
        return foundUser;
    }

    public User GetUserByUsername(string username)
    {
        User foundUser = null;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Username = @u";
                cmd.Parameters.AddWithValue("@u", username);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) foundUser = ParseUser(reader);
                }
            }
        }
        return foundUser;
    }

    public void AddWin(string username)
    {
        ExecuteQuery("UPDATE Users SET Wins = Wins + 1 WHERE Username = @u", username);
    }

    public void AddLoss(string username)
    {
        ExecuteQuery("UPDATE Users SET Losses = Losses + 1 WHERE Username = @u", username);
    }

    public List<User> GetTopPlayers(int count)
    {
        List<User> list = new List<User>();
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM Users ORDER BY Wins DESC LIMIT {count}";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(ParseUser(reader));
                    }
                }
            }
        }
        return list;
    }

    private User ParseUser(IDataReader reader)
    {
        string u = reader["Username"].ToString();
        string p = reader["Password"].ToString(); 
        User user = new User(u, p);

        user.Wins = System.Convert.ToInt32(reader["Wins"]);
        user.Losses = System.Convert.ToInt32(reader["Losses"]);
        return user;
    }

    private void ExecuteQuery(string query, string username)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@u", username);
                cmd.ExecuteNonQuery();
            }
        }
    }
}