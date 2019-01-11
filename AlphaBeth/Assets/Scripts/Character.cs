using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public delegate void CharacterDeathDelegate();

    //Events
    public event CharacterDeathDelegate DeathEvent;


    //Should be splitted into a healbar script? (IDamageable)
    public void Die()
    {
        //We don't track the death state or anything right now.
        if (DeathEvent != null)
            DeathEvent();
    }
}
