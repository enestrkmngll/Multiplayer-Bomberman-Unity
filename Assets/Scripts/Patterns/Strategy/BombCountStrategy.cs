using UnityEngine;
public class BombCountStrategy : IPowerUpStrategy
{
    private int amount;

    public BombCountStrategy(int amount)
    {
        this.amount = amount;
    }

    public void Apply(PlayerPresenter player)
    {
        player.IncreaseMaxBombs(amount);
        Debug.Log("Bomba Kapasitesi Artýrýldý!");
    }
}