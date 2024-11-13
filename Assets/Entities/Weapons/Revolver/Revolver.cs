using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Revolver : Weapon
{
    protected override void TriggerFireHitscan(InputAction.CallbackContext ctx)
    {
        base.TriggerFireHitscan(ctx);
        GetComponent<Animator>().SetTrigger("Shooting");
        //GetComponent<Animator>().SetBool("Shooting", false);
    }
}
