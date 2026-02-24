using UnityEngine;

public interface IExplosionObserver
{
    bool OnExplosionHit(Vector3Int gridPosition);
}