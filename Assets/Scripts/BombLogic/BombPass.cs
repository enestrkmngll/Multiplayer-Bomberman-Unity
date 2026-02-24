using UnityEngine;

public class BombPass : MonoBehaviour
{
    private Collider2D bombCollider;

    void Start()
    {
        bombCollider = GetComponent<Collider2D>();
        bombCollider.isTrigger = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        bombCollider.isTrigger = false;
    }
}