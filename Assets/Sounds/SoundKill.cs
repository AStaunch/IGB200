using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundKill : MonoBehaviour
{
    public float lifeTime = 3;

    // Use this for initialization
    void Start()
    {
        if (lifeTime != 0)
        {
            Destroy(this.gameObject, lifeTime);
        }
    }
}
