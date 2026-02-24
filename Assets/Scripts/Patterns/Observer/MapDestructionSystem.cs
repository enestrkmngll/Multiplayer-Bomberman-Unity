using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;

public class MapDestructionSystem : NetworkBehaviour, IExplosionObserver
{
    public Tilemap duvarTilemap;

    [Header("Power Up Ayarlarý")]
    public GameObject[] powerUpPrefabs;
    [Range(0, 100)]
    public int dropChance = 30;

    public enum WallType { None, Breakable, Hard, Unbreakable }

    public override void OnNetworkSpawn()
    {
        if (duvarTilemap == null)
        {
            FindTilemapReference();
        }
    }

    private void FindTilemapReference()
    {

        GameObject wallObj = GameObject.Find("Walls");

        if (wallObj != null)
        {
            duvarTilemap = wallObj.GetComponent<Tilemap>();
            Debug.Log($"[MapSystem] Tilemap baþarýyla bulundu: {wallObj.name}");
        }
        else
        {
            Tilemap[] tumHaritalar = FindObjectsOfType<Tilemap>();
            foreach (var harita in tumHaritalar)
            {
                if (harita.gameObject.name != "Ground")
                {
                    duvarTilemap = harita;
                    Debug.Log($"[MapSystem] Alternatif Tilemap bulundu: {harita.name}");
                    break;
                }
            }
        }
    }

    [ClientRpc]
    private void UpdateTileClientRpc(Vector3Int gridPos, string actionType)
    {
        if (duvarTilemap == null) FindTilemapReference();

        if (duvarTilemap == null)
        {
            Debug.LogError("Client Tilemap'i bulamadý, iþlem iptal.");
            return;
        }


        if (actionType == "Destroy")
        {
            duvarTilemap.SetTile(gridPos, null);
        }
        else if (actionType == "Degrade")
        {
            if (GameManager.Instance != null && GameManager.Instance.activeTheme != null)
            {
                ThemeFactory tema = GameManager.Instance.activeTheme;
                duvarTilemap.SetTile(gridPos, tema.GetBreakableWall());
            }
            else
            {
                Debug.LogWarning("Client temayý bilmediði için duvarý dönüþtüremedi!");
            }
        }

        duvarTilemap.RefreshTile(gridPos);
    }

    public WallType GetWallType(Vector3Int gridPos)
    {
        if (duvarTilemap == null) return WallType.None; 

        TileBase tile = duvarTilemap.GetTile(gridPos);
        if (tile == null) return WallType.None;

        if (GameManager.Instance == null || GameManager.Instance.activeTheme == null) return WallType.None;

        ThemeFactory tema = GameManager.Instance.activeTheme;
        string tileName = tile.name.Replace("(Clone)", "").Trim();
        string breakableName = tema.GetBreakableWall().name.Replace("(Clone)", "").Trim();
        string hardName = tema.GetHardWall().name.Replace("(Clone)", "").Trim();
        string unbreakableName = tema.GetUnbreakableWall().name.Replace("(Clone)", "").Trim();

        if (tileName == breakableName) return WallType.Breakable;
        if (tileName == hardName) return WallType.Hard;
        if (tileName == unbreakableName) return WallType.Unbreakable;

        return WallType.None;
    }

    public bool OnExplosionHit(Vector3Int gridPos)
    {
        if (!IsServer) return false;

        if (duvarTilemap == null) FindTilemapReference();

        WallType type = GetWallType(gridPos);

        if (type == WallType.Breakable)
        {
            TrySpawnPowerUpServerOnly(duvarTilemap.GetCellCenterWorld(gridPos));
            duvarTilemap.SetTile(gridPos, null);
            UpdateTileClientRpc(gridPos, "Destroy");
            return true;
        }
        else if (type == WallType.Hard)
        {
            ThemeFactory tema = GameManager.Instance.activeTheme;
            duvarTilemap.SetTile(gridPos, tema.GetBreakableWall());
            UpdateTileClientRpc(gridPos, "Degrade");
            return true;
        }
        return false;
    }

    private void TrySpawnPowerUpServerOnly(Vector3 position)
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;
        if (Random.Range(0, 100) < dropChance)
        {
            int index = Random.Range(0, powerUpPrefabs.Length);
            GameObject powerUp = Instantiate(powerUpPrefabs[index], position, Quaternion.identity);
            NetworkObject netObj = powerUp.GetComponent<NetworkObject>();
            if (netObj != null) netObj.Spawn();
        }
    }
}