using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ThemeUIManager : MonoBehaviour
{
    public GameObject menuPanel;

    [Header("UI Metinleri")]
    public TextMeshProUGUI currentUserStatsText;
    public TextMeshProUGUI leaderboardText;

    public ThemeFactory colTemasi;
    public ThemeFactory ormanTemasi;
    public ThemeFactory sehirTemasi;

    private GameStarterFacade gameFacade;
    private UserRepository userRepo;

    void Awake()
    {
        userRepo = new UserRepository();
    }

    void Start()
    {
        gameFacade = FindObjectOfType<GameStarterFacade>();
        if (gameFacade != null) gameFacade.Setup(menuPanel);

        UpdateLeaderboard();
    }

    public void SetUserInfo(User user)
    {
        Debug.Log("3. SetUserInfo çalýþtý. Gelen kullanýcý: " + user.Username);

        if (userRepo == null) userRepo = new UserRepository(); 

        User tazeUser = userRepo.GetUserByUsername(user.Username);

        if (tazeUser != null)
        {
            if (GameManager.Instance != null) GameManager.Instance.currentUser = tazeUser;

            if (currentUserStatsText != null)
            {
                currentUserStatsText.text = $"{tazeUser.Username}\nWin: {tazeUser.Wins} | Loss: {tazeUser.Losses}";
                Debug.Log("4. BAÞARILI! Ekrana þu yazýldý: " + currentUserStatsText.text);
            }
            else
            {

                Debug.LogError("HATA: Inspector'da 'Current User Stats Text' kutusuna Text'i sürüklememiþsin!");
            }
        }
        else
        {
            Debug.LogError("HATA: Veritabanýndan taze kullanýcý çekilemedi!");
        }

        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
        if (leaderboardText != null && userRepo != null)
        {
            List<User> topPlayers = userRepo.GetTopPlayers(3);
            string boardString = "<size=120%>TOP 3</size>\n\n";

            for (int i = 0; i < topPlayers.Count; i++)
            {
                boardString += $"{i + 1}. {topPlayers[i].Username} ({topPlayers[i].Wins} Win)\n";
            }
            leaderboardText.text = boardString;
        }
    }

    public void ColSecildi() => Baslat(colTemasi);
    public void OrmanSecildi() => Baslat(ormanTemasi);
    public void SehirSecildi() => Baslat(sehirTemasi);

    public void OyunaKatilButonu() { if (gameFacade != null) gameFacade.JoinGame(); CloseMenu(); }
    void Baslat(ThemeFactory t) { if (gameFacade != null) gameFacade.StartGame(t); CloseMenu(); }
    void CloseMenu() { if (menuPanel != null) menuPanel.SetActive(false); }
}