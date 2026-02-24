using System;

[Serializable]
public class User
{
    public string Username;
    public string Password;
    public int Wins;
    public int Losses;

    public User(string username, string password)
    {
        this.Username = username;
        this.Password = password;
        this.Wins = 0;
        this.Losses = 0;
    }
}