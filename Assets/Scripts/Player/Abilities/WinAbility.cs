using System.Collections;
using System.Collections.Generic;
using MajorJam.System;
using Systems;
using UnityEngine;

[CreateAssetMenu(menuName = "Cheats/End Game", fileName = "End Game Ability", order = 0)]
public class WinAbility : Ability
{
    

    protected override void OnAbilityUse(PlayerController playerController)
    {
        if (CanUseAbility)
        {
            UIManager.Get.WinGame();
            CanUseAbility = false;
        }
    }
}
