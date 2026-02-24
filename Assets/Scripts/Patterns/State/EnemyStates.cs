using UnityEngine;

public class WanderState : IEnemyState
{
    public void Enter(EnemyPresenter enemy)
    {
        Debug.Log("Düþman: Dolaþma Moduna (Wander) geçti.");
    }

    public Vector3 CalculateNextMove(EnemyPresenter enemy)
    {
        Transform target = enemy.FindClosestPlayer();
        if (target != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, target.position);

            if (distance < 5.0f)
            {
                enemy.ChangeState(new ChaseState());
                return enemy.transform.position;
            }
        }
        return enemy.GetRandomValidPosition();
    }

    public void Exit(EnemyPresenter enemy) { }
}

public class ChaseState : IEnemyState
{
    public void Enter(EnemyPresenter enemy)
    {
        Debug.Log("Düþman: KOVALAMA Moduna (Chase) geçti!");
    }

    public Vector3 CalculateNextMove(EnemyPresenter enemy)
    {
        Transform target = enemy.FindClosestPlayer();

        if (target == null || Vector3.Distance(enemy.transform.position, target.position) > 7.0f)
        {
            enemy.ChangeState(new WanderState());
            return enemy.transform.position;
        }

        Vector3 direction = (target.position - enemy.transform.position).normalized;
        Vector3 currentPos = enemy.transform.position;
        Vector3 intendedPos;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            intendedPos = currentPos + (direction.x > 0 ? Vector3.right : Vector3.left);
        }
        else
        {
            intendedPos = currentPos + (direction.y > 0 ? Vector3.up : Vector3.down);
        }

        if (enemy.IsPositionValid(intendedPos))
        {
            return intendedPos;
        }
        else
        {
            return enemy.GetRandomValidPosition();
        }
    }

    public void Exit(EnemyPresenter enemy) { }
}