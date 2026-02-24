using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyPresenter enemy);

    Vector3 CalculateNextMove(EnemyPresenter enemy);

    void Exit(EnemyPresenter enemy);
}