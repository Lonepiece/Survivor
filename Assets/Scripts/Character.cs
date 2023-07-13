using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static float Speed
    {
        ///get
        ///{
        ///    float speed = 0f;
        ///
        ///    switch (GameManager.instance.playerId)
        ///    {
        ///        case 0:
        ///        case 3:
        ///            speed = 1.1f;
        ///            break;
        ///
        ///        case 1:
        ///        case 2:
        ///            speed = 1f;
        ///            break;
        ///    }
        ///
        ///    return speed;
        ///}
        
        get { return GameManager.instance.playerId == 0 ? 1.1f : 1f; }
    }

    public static float WeaponSpeed
    {
        get { return GameManager.instance.playerId == 1 ? 1.1f : 1f; }
    }

    public static float WeaponRate
    {
        get { return GameManager.instance.playerId == 1 ? 0.9f : 1f; }
    }

    public static float Damage
    {
        get { return GameManager.instance.playerId == 2 ? 1.1f : 1f; }
    }

    public static int Count
    {
        get { return GameManager.instance.playerId == 3 ? 1 : 0; }
    }
}
