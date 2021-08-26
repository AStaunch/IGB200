using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;

public class TestSpell : MonoBehaviour
{
    SpellEffector Test = new SpellEffector() { DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"), Name = "Test",
        FireEffect = new Action<GameObject>((gmeobj) => {
            Debug.Log($"Test Spell Effector recieved: {gmeobj.transform.position.x} - {gmeobj.transform.position.y}");

            //Fire Properties
            //float baseDmg = 10f;
            //if (gmeobj.TryGetComponent<EntityManager>(out EntityManager otherEntity))
            //{
            //    if (otherEntity.entityProperties.Contains(Properties.Flamable))
            //    {
            //        otherEntity.TakeDamage(baseDmg * 2f);
            //    }
            //    else if (otherEntity.entityProperties.Contains(Properties.Fireproof))
            //    {
            //        otherEntity.TakeDamage(0f);
            //    }
            //    else
            //    {
            //        otherEntity.TakeDamage(baseDmg);
            //    }
            //}

            Destroy(gmeobj);
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
