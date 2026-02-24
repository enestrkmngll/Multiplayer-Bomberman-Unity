using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;


public class EnemySpawner : NetworkBehaviour
{
    private MapGenerator mapGenerator;

    void Awake()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    public void DüsmanlariOlustur(ThemeFactory currentTheme)
    {
        Debug.Log("1. DüsmanlariOlustur fonksiyonuna giriþ yapýldý.");

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("UYARI: Server/Host deðilim (veya NetworkManager yok). Ýþlem iptal.");
            return;
        }
        // ----------------------------------

        if (mapGenerator == null)
        {
            Debug.LogError("HATA: MapGenerator bulunamadý!");
            return;
        }

        if (mapGenerator.enemySpawnPoints == null || mapGenerator.enemySpawnPoints.Count == 0)
        {
            Debug.LogWarning("UYARI: Düþman doðacak nokta (enemySpawnPoints) boþ! Harita henüz oluþmamýþ olabilir.");
            return;
        }

        GameObject enemyPrefab = currentTheme.GetEnemyPrefab();
        if (enemyPrefab == null)
        {
            Debug.LogError("HATA: ThemeFactory düþman prefabý veremedi!");
            return;
        }

        int count = 0;
        foreach (Vector3 spawnPos in mapGenerator.enemySpawnPoints)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            NetworkObject netObj = newEnemy.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn(); 
                count++;
            }
            else
            {
                Debug.LogError($"HATA: {newEnemy.name} üzerinde NetworkObject yok!");
            }
        }

        Debug.Log($"BAÞARILI: Server toplam {count} düþman oluþturdu.");
    }
}