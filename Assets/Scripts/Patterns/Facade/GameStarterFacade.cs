using UnityEngine;
using Unity.Netcode;
using System.Collections; 

public class GameStarterFacade : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    private GameObject menuPanel;
    private ThemeNetworkManager themeNetworkManager;
    private MapGenerator mapGenerator; 

    void Awake()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        themeNetworkManager = FindObjectOfType<ThemeNetworkManager>();
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    public void Setup(GameObject menu)
    {
        this.menuPanel = menu;
    }

    public void JoinGame()
    {
        Debug.Log("Client olarak baðlanýlýyor...");
        bool basarili = NetworkManager.Singleton.StartClient();
        if (basarili && menuPanel != null) menuPanel.SetActive(false);
    }

    public void StartGame(ThemeFactory theme)
    {
        Debug.Log("Oyun Baþlatýlýyor: " + theme.name);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameData();
        }

        GameManager.Instance.activeTheme = theme;

        bool serverStarted = NetworkManager.Singleton.StartHost();

        if (serverStarted)
        {
            Debug.Log("Host Baþlatýldý");

            if (themeNetworkManager != null)
            {
                themeNetworkManager.SetTheme(theme);
            }

            StartCoroutine(SpawnEnemiesWithDelay(theme));

            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }
        }
    }

    private IEnumerator SpawnEnemiesWithDelay(ThemeFactory theme)
    {
        yield return new WaitForSeconds(0.1f);

        if (enemySpawner != null)
        {
            enemySpawner.DüsmanlariOlustur(theme);
        }

        if (NetworkManager.Singleton.SpawnManager != null && NetworkManager.Singleton.LocalClient != null)
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;

            while (localPlayer == null)
            {
                yield return new WaitForSeconds(0.1f);
                if (NetworkManager.Singleton.LocalClient != null)
                    localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
            }

            if (mapGenerator != null)
            {
                localPlayer.transform.position = mapGenerator.player1SpawnPoint;
                Debug.Log("Oyuncu baþlangýç noktasýna taþýndý.");
            }
        }
    }
}