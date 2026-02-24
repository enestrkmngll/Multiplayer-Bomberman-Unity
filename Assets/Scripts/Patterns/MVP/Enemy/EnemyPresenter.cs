using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemyPresenter : NetworkBehaviour, IExplosionObserver
{
    public EnemyModel model = new EnemyModel();

    private EnemyView view;

    private IEnemyState currentState;

    private NetworkVariable<bool> netIsMoving = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        view = GetComponent<EnemyView>();
        netIsMoving.OnValueChanged += OnMovingStateChanged;

        if (IsServer)
        {
            if (GameManager.Instance != null) GameManager.Instance.RegisterEnemy();


            ChangeState(new WanderState());

            StartCoroutine(MovementLoop());
        }
        else
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = true;
        }
    }

    public override void OnNetworkDespawn()
    {
        netIsMoving.OnValueChanged -= OnMovingStateChanged;
    }

    private void OnMovingStateChanged(bool prev, bool current)
    {
        if (view != null) view.SetMovingState(current);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null) currentState.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    IEnumerator MovementLoop()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (model.isDead) break;

            Vector3 targetPosition = currentState.CalculateNextMove(this);

            if (targetPosition == transform.position)
            {
                netIsMoving.Value = false;
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            yield return StartCoroutine(MoveToTarget(targetPosition));
            yield return null;
        }
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        netIsMoving.Value = true;
        UpdateDirectionClientRpc(target);

        float elapsedTime = 0;
        Vector3 startPosition = transform.position;

        while (elapsedTime < model.moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / model.moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        netIsMoving.Value = false;
    }

    [ClientRpc]
    void UpdateDirectionClientRpc(Vector3 target)
    {
        if (view != null) view.UpdateDirection(target);
    }

    public Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(p.transform.position, currentPos);
            if (dist < minDistance)
            {
                closest = p.transform;
                minDistance = dist;
            }
        }
        return closest;
    }

    public Vector3 GetRandomValidPosition()
    {
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Floor(currentPos.x) + 0.5f;
        currentPos.y = Mathf.Floor(currentPos.y) + 0.5f;

        int attempts = 0;
        while (attempts < 10)
        {
            int r = Random.Range(0, 4);
            Vector3 dir = r == 0 ? Vector3.up : r == 1 ? Vector3.down : r == 2 ? Vector3.left : Vector3.right;
            Vector3 target = currentPos + dir;

            if (IsPositionValid(target)) return target;
            attempts++;
        }
        return transform.position; 
    }

    public bool IsPositionValid(Vector3 targetPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, model.collisionTolerance);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Wall") || hit.CompareTag("Indestructible") || hit.CompareTag("Bomb"))
            {
                return false;
            }
        }
        return true;
    }

    public bool OnExplosionHit(Vector3Int gridPosition)
    {
        if (!IsServer || model.isDead) return false;
        model.isDead = true;
        StartCoroutine(DieRoutine());
        return false;
    }

    IEnumerator DieRoutine()
    {
        StopAllCoroutines();
        if (view != null) view.PlayDeathAnimClientRpc();
        if (GameManager.Instance != null) GameManager.Instance.OnEnemyKilled();
        yield return new WaitForSeconds(1f);
        if (IsServer && IsSpawned) NetworkObject.Despawn();
    }
}