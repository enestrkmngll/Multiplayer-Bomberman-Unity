using UnityEngine;
using Unity.Netcode;

public class EnemyView : NetworkBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMovingState(bool isMoving)
    {
        if (anim != null)
        {
            anim.SetBool("isMoving", isMoving);
        }
    }

    public void UpdateDirection(Vector3 targetPos)
    {

        if (spriteRenderer == null) return;

        if (targetPos.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (targetPos.x < transform.position.x)
            spriteRenderer.flipX = true;
    }

    [ClientRpc]
    public void PlayDeathAnimClientRpc()
    {
        if (anim != null) anim.SetTrigger("Die");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}