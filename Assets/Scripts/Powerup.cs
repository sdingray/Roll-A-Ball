using System.Collections;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    //An enum is a datatype that we can specify its values and use
    public enum PowerupType {SpeedUp, SpeedDown}

    public PowerupType myPowerup;           //This objects powerup type
    public float powerupDuration = 7f;      //The duration of the power up
    PlayerController playerController;      //A reference to our player controller

    void Start()
    {
        //Find and assign the player controller object to this loacal reference
        playerController = FindObjectOfType<PlayerController>();
    }

    public void UsePowerup()
    {
        //If this powerup is the speedup powerup, increase the player controller speed by double (I MADE IT 10)
        if (myPowerup == PowerupType.SpeedUp)
            playerController.speed = playerController.baseSpeed * 10;

        //If this powerup is the speeddown powerup, decrease the player controller speed times 3
        if (myPowerup == PowerupType.SpeedDown)
            playerController.speed = playerController.baseSpeed / 3;

        //Start a coroutine to reset the powerups effects
        StartCoroutine(ResetPowerup());
    }

    IEnumerator ResetPowerup()
    {
        yield return new WaitForSeconds(powerupDuration);

        //If this powerup relates to speed, reset our playuer controller speed to its base speed
        if(myPowerup == PowerupType.SpeedUp || myPowerup == PowerupType.SpeedDown)
            playerController.speed = playerController.baseSpeed;
    }
}
