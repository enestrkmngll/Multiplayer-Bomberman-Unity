using UnityEngine;
using Unity.Netcode;

public enum PowerUpType
{
    Speed,      
    Range,      
    BombCount   
}

public class PowerUpItem : NetworkBehaviour 
{
    [Header("Ayarlar")]
    public PowerUpType powerUpType;

    public float value = 1f;

    [Header("Süre Ayarý")]
    public float duration = 5f;

    private IPowerUpStrategy _strategy;

    void Start()
    {
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                _strategy = new SpeedBoostStrategy(value, duration);
                break;
            case PowerUpType.Range:
                _strategy = new RangeBoostStrategy((int)value);
                break;
            case PowerUpType.BombCount:
                _strategy = new BombCountStrategy((int)value);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            PlayerPresenter player = other.GetComponent<PlayerPresenter>();

            if (player != null)
            {
                if (_strategy != null)
                {
                    _strategy.Apply(player);
                }

                NetworkObject netObj = GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    netObj.Despawn();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}