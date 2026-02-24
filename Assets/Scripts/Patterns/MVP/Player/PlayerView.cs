using UnityEngine;
using Unity.Netcode;

public class PlayerView : NetworkBehaviour
{
    private PlayerPresenter presenter;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    private bool isDead = false;

    void Start()
    {
        presenter = GetComponent<PlayerPresenter>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {

        if (isDead || !IsOwner) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (presenter != null)
        {
            presenter.OnMoveInput(x, y);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                presenter.OnBombPlaceInput();
            }
        }
    }

    public void MovePlayer(Vector2 velocity)
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = velocity;

        if (animator != null)
        {
            bool hareketEdiyorMu = velocity.sqrMagnitude > 0.1f;
            animator.SetBool("IsMoving", hareketEdiyorMu);

            if (hareketEdiyorMu)
            {
                animator.SetFloat("InputX", velocity.x);
                animator.SetFloat("InputY", velocity.y);
            }
        }
    }

    public void PlayDeathAnimation()
    {
        if (animator != null) animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
    }


}