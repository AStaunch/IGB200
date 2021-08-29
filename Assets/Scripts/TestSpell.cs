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
        Effector = new Action<GameObject>((gmeobj) => {
            Debug.Log($"Test Spell Fire recieved: {gmeobj.transform.position.x} - {gmeobj.transform.position.y}");

            //Fire Properties
            float baseDmg = 10f;
            if (gmeobj.TryGetComponent<EntityManager>(out EntityManager otherEntity)) {
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
            Color.red,
            Color.red,
            Color.red,
            Color.yellow
        }
    };

    SpellEffector Pull = new SpellEffector() {
        DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Pull",
        Effector = new Action<GameObject>((gmeobj) => {
            Debug.Log($"Test Spell Pull recieved: {gmeobj.transform.position.x} - {gmeobj.transform.position.y}");

            //Pull Properties
            float baseDmg = 10f;
            float commonFactor = 10f;
            float force = baseDmg * commonFactor;
            Vector2 facing = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();

            if (gmeobj.TryGetComponent<EntityManager>(out EntityManager otherEntity)) {
                if (otherEntity.entityProperties.Contains(Properties.Light)) {
                    gmeobj.GetComponent<Rigidbody2D>().AddForce(1.5f * force * facing);
                } else if (otherEntity.entityProperties.Contains(Properties.Heavy)) {
                    gmeobj.GetComponent<Rigidbody2D>().AddForce(0.2f * force * facing);
                } else {
                    gmeobj.GetComponent<Rigidbody2D>().AddForce(facing * force);
                }
            }
        }),
        Colors = new Color[]{
            Color.magenta,
            Color.magenta,
            Color.magenta,
            Color.yellow
        }
    };

    SpellEffector Push = new SpellEffector() {
        DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Push",
        Effector = new Action<GameObject>((gmeobj) => {
            Debug.Log($"Test Spell Effector recieved: {gmeobj.transform.position.x} - {gmeobj.transform.position.y}");

            //Push Properties
            float baseDmg = 10f;
            float commonFactor = 10f;
            float force = -baseDmg * commonFactor;
            Vector2 facing = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();

            if (gmeobj.TryGetComponent<EntityManager>(out EntityManager otherEntity)) {
                if (otherEntity.entityProperties.Contains(Properties.Light)) {
                    gmeobj.GetComponent<Rigidbody2D>().AddForce(1.5f * force * facing);
                } else if (otherEntity.entityProperties.Contains(Properties.Heavy)) {
                    gmeobj.GetComponent<Rigidbody2D>().AddForce(0.2f * force * facing);
                } else {
                    gmeobj.GetComponent<Rigidbody2D>().AddForce(facing * force);
                }
            }
        }),
        Colors = new Color[]{
            Color.yellow,
            Color.yellow,
            Color.yellow,
            Color.magenta
        }
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Fire);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Pull);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Push);
    }
}
