using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Netcode;

public class MapGenerator : NetworkBehaviour
{
    [Header("Tilemap Referanslarý")]
    public Tilemap zeminTilemap;
    public Tilemap duvarTilemap;

    public List<Vector3> enemySpawnPoints { get; private set; }
    public Vector3 player1SpawnPoint { get; private set; }
    public Vector3 player2SpawnPoint { get; private set; }

    public void GenerateLevel(ThemeFactory theme)
    {
        Debug.Log("Harita Çizim Emri Alýndý: " + theme.name);

        DrawMapLocal(theme);

        
    }

    private void DrawMapLocal(ThemeFactory theme)
    {
        zeminTilemap.ClearAllTiles();
        duvarTilemap.ClearAllTiles();
        enemySpawnPoints = new List<Vector3>();

        string[] levelMap = {
            "11111111111111111",
            "100032332332300E1",
            "10112221112221101",
            "10122221012222101",
            "13221300200312231",
            "12211022322011221",
            "13230021012003231",
            "10202130E03120201",
            "13230021012003231",
            "12211022322011221",
            "13221300200312231",
            "10122221012222101",
            "10112221112221101",
            "1E003233233230001",
            "11111111111111111"
        };

        int height = levelMap.Length;
        int width = levelMap[0].Length;

        player1SpawnPoint = new Vector3(1.5f, (height - 1 - 1) + 0.5f, 0);
        player2SpawnPoint = new Vector3((width - 2) + 0.5f, 1.5f, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                char tileType = levelMap[y][x];
                Vector3Int pos = new Vector3Int(x, height - 1 - y, 0);

                zeminTilemap.SetTile(pos, theme.GetGround());

                switch (tileType)
                {
                    case '1': duvarTilemap.SetTile(pos, theme.GetUnbreakableWall()); break;
                    case '2': duvarTilemap.SetTile(pos, theme.GetBreakableWall()); break;
                    case '3': duvarTilemap.SetTile(pos, theme.GetHardWall()); break;
                    case 'E': enemySpawnPoints.Add(zeminTilemap.GetCellCenterWorld(pos)); break;
                }
            }
        }
    }
}