using UnityEngine;
using Unity.Netcode;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerView))]
public class PlayerPresenter : NetworkBehaviour, IExplosionObserver
{
    [Header("Baþlangýç Ayarlarý")]
    public PlayerModel model; 


    public NetworkVariable<float> netMoveSpeed = new NetworkVariable<float>(5f);
    public NetworkVariable<int> netExplosionRange = new NetworkVariable<int>(1);
    public NetworkVariable<int> netMaxBombs = new NetworkVariable<int>(1);

    private int activeBombs = 0;

    [Header("Ayarlar")]
    public GameObject bombPrefab;

    private PlayerView view;

    void Start()
    {
        if (model == null) model = new PlayerModel();
        view = GetComponent<PlayerView>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            netMoveSpeed.Value = model.moveSpeed;
            netExplosionRange.Value = model.explosionRange;
            netMaxBombs.Value = model.maxBombs;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterPlayer();
                Debug.Log($"[PlayerPresenter] Oyuncu Eklendi! ID: {OwnerClientId}");
            }
            else
            {
                Debug.LogError("[PlayerPresenter] HATA: GameManager bulunamadý! Oyuncu sayýlamadý.");

                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.RegisterPlayer();
                    Debug.Log("[PlayerPresenter] GameManager sonradan bulundu ve kayýt yapýldý.");
                }
            }
        }

        if (IsOwner)
        {
            MapGenerator mapGen = FindObjectOfType<MapGenerator>();
            if (mapGen != null)
            {
                ulong myId = OwnerClientId;
                if (myId == 0) transform.position = mapGen.player1SpawnPoint;
                else transform.position = mapGen.player2SpawnPoint;
            }
        }
    }

    public void OnMoveInput(float x, float y)
    {
        if (!IsOwner) return;


        Vector2 movement = new Vector2(x, y) * netMoveSpeed.Value;
        view.MovePlayer(movement);
    }

    public void OnBombPlaceInput()
    {
        if (!IsOwner) return;

        if (activeBombs < netMaxBombs.Value)
        {
            activeBombs++;
            RequestBombServerRpc();
        }
        else
        {
            Debug.Log("Bomba hakkýn bitti!");
        }
    }

    public void OnBombExploded()
    {
        if (IsServer)
        {
            ReplenishBombClientRpc();
        }
    }

    [ClientRpc]
    private void ReplenishBombClientRpc()
    {
        if (IsOwner)
        {
            activeBombs--;
            if (activeBombs < 0) activeBombs = 0;
        }
    }

    [ServerRpc]
    void RequestBombServerRpc()
    {
        Vector3 bombPosition;

        if (GameManager.Instance != null && GameManager.Instance.duvarTilemap != null)
        {
            UnityEngine.Tilemaps.Tilemap harita = GameManager.Instance.duvarTilemap;
            Vector3Int hucreKonumu = harita.WorldToCell(transform.position);
            bombPosition = harita.GetCellCenterWorld(hucreKonumu);
        }
        else
        {
            bombPosition = new Vector3(
                Mathf.Floor(transform.position.x) + 0.5f,
                Mathf.Floor(transform.position.y) + 0.5f,
                0
            );
        }

        GameObject yeniBomba = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
        Bomb bombaScripti = yeniBomba.GetComponent<Bomb>();

        if (bombaScripti != null)
        {
            bombaScripti.patlamaMenzili = netExplosionRange.Value;
            bombaScripti.owner = this;
        }

        yeniBomba.GetComponent<NetworkObject>().Spawn();
    }


    public void IncreaseSpeed(float amount)
    {
        if (IsServer) netMoveSpeed.Value += amount;
    }

    public void IncreaseExplosionRange(int amount)
    {
        if (IsServer) netExplosionRange.Value += amount;
    }

    public void IncreaseMaxBombs(int amount)
    {
        if (IsServer) netMaxBombs.Value += amount;
    }

    public void BoostSpeedTemporarily(float amount, float duration)
    {
        if (IsServer) StartCoroutine(SpeedDurationRoutine(amount, duration));
    }

    private IEnumerator SpeedDurationRoutine(float amount, float duration)
    {
        netMoveSpeed.Value += amount;

        yield return new WaitForSeconds(duration);

        netMoveSpeed.Value -= amount;
    }

    public bool OnExplosionHit(Vector3Int gridPosition)
    {
        if (!IsServer) return false;

        Debug.Log("Presenter: Vuruldum! Karar veriyorum...");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied(OwnerClientId);
        }

        DieClientRpc();

        return false;
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        view.PlayDeathAnimation();

        this.enabled = false;
    }
}