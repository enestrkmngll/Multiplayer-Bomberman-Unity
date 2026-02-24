using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Oyun Takibi")]
    public int livingEnemyCount = 0;
    public int livingPlayerCount = 0;

    private bool localPlayerHasLost = false;

    [Header("Harita Referanslarý")]
    public UnityEngine.Tilemaps.Tilemap duvarTilemap;
    public ThemeFactory activeTheme;

    private UserRepository userRepo;
    public User currentUser;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            userRepo = new UserRepository();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameData();
    }

    public void ResetGameData()
    {
        livingEnemyCount = 0;
        livingPlayerCount = 0;
        localPlayerHasLost = false;
        Debug.Log("GameManager: Veriler Sýfýrlandý (Yeni Oyun Hazýr)");
    }

    public void RegisterEnemy() => livingEnemyCount++;
    public void RegisterPlayer() => livingPlayerCount++;

    public void OnEnemyKilled()
    {
        if (!IsServer) return;
        livingEnemyCount--;
        CheckWinCondition();
    }

    public void OnPlayerDied(ulong loserClientId)
    {
        if (!IsServer) return;
        livingPlayerCount--;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { loserClientId } }
        };


        ProcessResultClientRpc(false, "OYUN BÝTTÝ\nKAYBETTÝN!", Color.red, clientRpcParams);

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (livingEnemyCount <= 0 && livingPlayerCount > 0)
        {
            AnnounceWinnerClientRpc();
        }
    }


    [ClientRpc]
    private void ProcessResultClientRpc(bool isWinner, string msg, Color color, ClientRpcParams clientRpcParams = default)
    {
        if (!isWinner)
        {
            localPlayerHasLost = true;
        }

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.ShowGameOver(msg, color);

        SaveToDatabase(isWinner);
    }

    [ClientRpc]
    private void AnnounceWinnerClientRpc()
    {
        if (!localPlayerHasLost)
        {
            Debug.Log("Kazandýn UI tetikleniyor...");
            Color winColor = Color.white; 

            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowGameOver("TEBRÝKLER!\nKAZANDIN!", winColor);
            }

            SaveToDatabase(true);
        }
    }

    private void SaveToDatabase(bool isWinner)
    {
        if (currentUser == null || string.IsNullOrEmpty(currentUser.Username))
        {
            Debug.LogWarning("Veritabanýna kayýt yapýlamadý: Kullanýcý Giriþi Yok.");
            return;
        }

        if (userRepo == null) userRepo = new UserRepository();

        try
        {
            if (isWinner)
            {
                userRepo.AddWin(currentUser.Username);
                currentUser.Wins++; 
            }
            else
            {
                userRepo.AddLoss(currentUser.Username);
                currentUser.Losses++; 
            }
            Debug.Log($"Veritabaný Güncellendi: {currentUser.Username} (Win/Loss)");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Veritabaný Kayýt Hatasý: {e.Message}");
        }
    }
}