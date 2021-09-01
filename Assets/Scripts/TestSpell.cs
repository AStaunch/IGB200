using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;

public class TestSpell : MonoBehaviour
{
    SpellEffector Fire = new SpellEffector() {
        DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Fire",
        Effector = new Action<EffectorData>((EffectorData_) => {
            RayData Ray_ = (RayData)EffectorData_;
            Debug.Log($"Test Spell Fire recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

            /*Fire Properties
             * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
             * */
            float baseDmg = 10f;
            Debug.Log(Ray_.Data.collider.transform.name + " was hit.");
            if (Ray_.Data.collider.gameObject.TryGetComponent<EntityManager>(out EntityManager otherEntity)) {
                if (otherEntity.entityProperties.Contains(Properties.Flamable)) {
                    otherEntity.TakeDamage(baseDmg * 2f);
                } else if (otherEntity.entityProperties.Contains(Properties.Fireproof)) {
                    otherEntity.TakeDamage(0f);
                } else {
                    otherEntity.TakeDamage(baseDmg);
                }
            }
        }),
        Colors = new Color[]{
                new Color(0.5411f,0.1216f,0.07451f,1f),
                new Color(0.9176f,0.07451f,0.0039f,1f),
                new Color(1f,0.3843f,0f,1f),
                new Color(0.9686f,0.6824f,0.1765f,1f)
        }
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Fire);
    }
}
