using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpell : MonoBehaviour
{
    SpellEffector Test = new SpellEffector() { DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"), Name = "Test",
        FireEffect = new Action<GameObject>((gmeobj) => {
            Debug.Log($"Test Spell Effector recieved: {gmeobj.transform.position.x} - {gmeobj.transform.position.y}");
        })
    };

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Test);
    }
}
