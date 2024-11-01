using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public Image cooldownTimer;
    public PlayerCombat player;
    public PlayerMovement playerMovement;

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = player.health / player.maxHealth;
        cooldownTimer.fillAmount = 1f - ((Mathf.Clamp(playerMovement.timeToNextDownDash, 0, playerMovement.downDashCooldown)) / playerMovement.downDashCooldown);
    }
}
