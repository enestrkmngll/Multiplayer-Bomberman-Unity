using UnityEngine;
using Unity.Netcode;

public class ExplosionArea : MonoBehaviour
{

    [HideInInspector]
    public bool hasarVurabilir = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        HasarVer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HasarVer(other);
    }

    void HasarVer(Collider2D other)
    {
        if (!hasarVurabilir) return;

        IExplosionObserver observer = other.GetComponent<IExplosionObserver>();

        if (observer != null)
        {
            Vector3Int pos = Vector3Int.RoundToInt(other.transform.position);
            observer.OnExplosionHit(pos);


        }
    }
}