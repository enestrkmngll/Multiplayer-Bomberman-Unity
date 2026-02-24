using UnityEngine;

public class RangeBoostStrategy : IPowerUpStrategy
{
    private int amount;

    public RangeBoostStrategy(int amount)
    {
        this.amount = amount;
    }

    public void Apply(PlayerPresenter player)
    {
        player.IncreaseExplosionRange(amount);
        Debug.Log("Patlama Menzili Artýrýldý!");
    }
}