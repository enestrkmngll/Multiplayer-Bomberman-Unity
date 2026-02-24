using UnityEngine;

public class SpeedBoostStrategy : IPowerUpStrategy
{
    private float speedAmount;
    private float duration;

    public SpeedBoostStrategy(float amount, float duration)
    {
        this.speedAmount = amount;
        this.duration = duration;
    }

    public void Apply(PlayerPresenter player)
    {
        float currentSpeed = player.netMoveSpeed.Value;

        player.BoostSpeedTemporarily(currentSpeed / 2, duration); 

        Debug.Log("Hýz boostlandý! (Server)");
    }
}