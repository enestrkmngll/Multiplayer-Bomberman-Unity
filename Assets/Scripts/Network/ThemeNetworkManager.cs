using UnityEngine;
using Unity.Netcode;

public class ThemeNetworkManager : NetworkBehaviour
{
    [Header("Tema Listesi (Database)")]
    public ThemeFactory[] availableThemes;

    [Header("Baðlantýlar")]
    public MapGenerator mapGenerator; 

    private NetworkVariable<int> currentThemeIndex = new NetworkVariable<int>(
        -1,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        if (mapGenerator == null) mapGenerator = FindObjectOfType<MapGenerator>();
    }

    public override void OnNetworkSpawn()
    {
        currentThemeIndex.OnValueChanged += OnThemeIndexChanged;


        if (IsClient && currentThemeIndex.Value != -1)
        {
            ApplyTheme(currentThemeIndex.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        currentThemeIndex.OnValueChanged -= OnThemeIndexChanged;
    }

    public void SetTheme(ThemeFactory theme)
    {
        if (!IsServer) return; 

        int index = GetThemeIndex(theme);
        if (index != -1)
        {
            currentThemeIndex.Value = index;
        }
        else
        {
            Debug.LogError("Seçilen tema ThemeNetworkManager listesinde yok!");
        }
    }

    private void OnThemeIndexChanged(int oldVal, int newVal)
    {
        ApplyTheme(newVal);
    }

    private void ApplyTheme(int index)
    {
        if (index >= 0 && index < availableThemes.Length)
        {
            ThemeFactory themeToLoad = availableThemes[index];

            if (GameManager.Instance != null)
            {
                GameManager.Instance.activeTheme = themeToLoad;
                Debug.Log($"[Client] Tema GameManager'a iþlendi: {themeToLoad.name}");
            }
            if (mapGenerator != null)
            {
                mapGenerator.GenerateLevel(themeToLoad);
            }
        }
    }

    private int GetThemeIndex(ThemeFactory theme)
    {
        for (int i = 0; i < availableThemes.Length; i++)
        {
            if (availableThemes[i] == theme) return i;
        }
        return -1;
    }
}