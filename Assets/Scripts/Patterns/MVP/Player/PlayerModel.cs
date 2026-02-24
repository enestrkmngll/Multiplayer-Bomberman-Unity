using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    [Header("Ýstatistikler")]
    public float moveSpeed = 5f;
    public int explosionRange = 1;
    public int maxBombs = 1;

    [HideInInspector] public int activeBombs = 0;
}