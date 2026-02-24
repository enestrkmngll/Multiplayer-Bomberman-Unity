using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : NetworkBehaviour
{
    [Header("Patlama Görselleri")]
    public GameObject explosionCenter;
    public GameObject explosionMiddle;
    public GameObject explosionEnd;

    [Header("Ayarlar")]
    public float patlamaSuresi = 2.5f;
    public int patlamaMenzili = 2;

    public PlayerPresenter owner;
    private MapDestructionSystem mapDestructionSystem;

    public override void OnNetworkSpawn()
    {
        mapDestructionSystem = FindObjectOfType<MapDestructionSystem>();
        if (IsServer) StartCoroutine(PatlamaSayaci());
    }

    IEnumerator PatlamaSayaci()
    {
        yield return new WaitForSeconds(patlamaSuresi);

        if (owner != null) owner.OnBombExploded();

        PatlamayiHesaplaVeGonder();

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(0.1f);

        if (IsServer && IsSpawned)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    private void PatlamayiHesaplaVeGonder()
    {
        SpawnExplosionVisualClientRpc(transform.position, Vector2.zero, "Center");

        HesaplaYondekiPatlama(Vector2.up);
        HesaplaYondekiPatlama(Vector2.down);
        HesaplaYondekiPatlama(Vector2.left);
        HesaplaYondekiPatlama(Vector2.right);
    }

    private void HesaplaYondekiPatlama(Vector2 yon)
    {
        Tilemap duvarTilemap = GameManager.Instance.duvarTilemap;

        for (int i = 1; i <= patlamaMenzili; i++)
        {
            Vector3 kontrolNoktasi = transform.position + (Vector3)(yon * i);
            Vector3Int gridPos = duvarTilemap.WorldToCell(kontrolNoktasi);

            var wallType = mapDestructionSystem.GetWallType(gridPos);

            string visualType = (i == patlamaMenzili) ? "End" : "Middle";

            if (wallType == MapDestructionSystem.WallType.None)
            {
                SpawnExplosionVisualClientRpc(kontrolNoktasi, yon, visualType);
            }
            else if (wallType == MapDestructionSystem.WallType.Breakable) 
            {
                mapDestructionSystem.OnExplosionHit(gridPos);

                SpawnExplosionVisualClientRpc(kontrolNoktasi, yon, visualType);

            }
            else if (wallType == MapDestructionSystem.WallType.Hard)
            {
                mapDestructionSystem.OnExplosionHit(gridPos);

                SpawnExplosionVisualClientRpc(kontrolNoktasi, yon, "End");

                break;
            }
            else if (wallType == MapDestructionSystem.WallType.Unbreakable)
            {
                break;
            }
        }
    }

    [ClientRpc]
    private void SpawnExplosionVisualClientRpc(Vector3 position, Vector2 yon, string type)
    {
        Tilemap harita = GameManager.Instance.duvarTilemap;
        Vector3Int gridPos = harita.WorldToCell(position);
        Vector3 gridCenter = harita.GetCellCenterWorld(gridPos);

        GameObject prefabToUse = explosionCenter;
        Quaternion rotasyon = Quaternion.identity;

        if (type == "Center") prefabToUse = explosionCenter;
        else
        {
            if (yon == Vector2.up) rotasyon = Quaternion.Euler(0, 0, 90);
            else if (yon == Vector2.down) rotasyon = Quaternion.Euler(0, 0, -90);
            else if (yon == Vector2.left) rotasyon = Quaternion.Euler(0, 0, 180);
            else if (yon == Vector2.right) rotasyon = Quaternion.Euler(0, 0, 0);

            prefabToUse = (type == "End") ? explosionEnd : explosionMiddle;
        }

        GameObject effect = Instantiate(prefabToUse, gridCenter, rotasyon);
        ExplosionArea area = effect.GetComponent<ExplosionArea>();
        if (area != null) area.hasarVurabilir = IsServer;
    }
}