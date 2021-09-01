using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;

public class AlexTestSpells : MonoBehaviour
{
    public enum Elements
    {
        Fire, Ice, Earth, Lightning, Pull, Push, Life, Death
    }
    static readonly Dictionary<Elements, Color[]> ColourDict = new Dictionary<Elements, Color[]>() {
        {Elements.Fire, new Color[]
            {
            new Color32(138,31,19,255),
            new Color32(234,19,1,255),
            new Color32(255,98,0,255),
            new Color32(247,174,45,255)
            }
        },
        {Elements.Ice, new Color[]
            {
            new Color32(39,92,68,255),
            new Color32(64,137,73,255),
            new Color32(100,199,77,255),
            new Color32(255,255,255,255)
            }
        },
        {Elements.Pull, new Color[]
            {
            new Color32(36,46,71,255),
            new Color32(104,56,108,255),
            new Color32(181,80,136,255),
            new Color32(246,117,122,255)
            }
        },
        {Elements.Push, new Color[]
            {
            new Color32(36,46,71,255),
            new Color32(181,80,136,255),
            new Color32(137,143,250,255),
            new Color32(104,56,108,255)
            }
        },
        {Elements.Life, new Color[]
            {
            new Color32(39,92,68,255),
            new Color32(64,137,73,255),
            new Color32(100,199,77,255),
            new Color32(255,255,255,255)
            }
        },
        {Elements.Death, new Color[]
            {
            new Color32(25,20,38,255),
            new Color32(40,43,68,255),
            new Color32(59,68,104,255),
            new Color32(90,104,136,255)
            }
        },
        {Elements.Earth, new Color[]
            {
            new Color32(26,59,62,255),
            new Color32(64,39,51,255),
            new Color32(117,62,57,255),
            new Color32(185,111,81,255)
            }
        },
        {Elements.Lightning, new Color[]
            {
            new Color32(248,118,35,255),
            new Color32(255,173,52,255),
            new Color32(254,230,98,255),
            new Color32(255,255,255,255)
            }
        }
    };

    SpellEffector Pull = new SpellEffector() {
        DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Pull",
        Effector = new Action<EffectorData>((EffectorData_) => {
            RayData Ray_ = (RayData)EffectorData_;
            Debug.Log($"Test Spell Pull Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

            /*Fire Properties
             * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
             * */
            float baseDmg = 10f;
            GameObject obj = Ray_.Data.collider.gameObject;
            ForceObject(obj, baseDmg);
        }),
        Colors = ColourDict[Elements.Pull]

    };

    SpellEffector Push = new SpellEffector() {
        DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Push",
        Effector = new Action<EffectorData>((EffectorData_) => {
            RayData Ray_ = (RayData)EffectorData_;
            Debug.Log($"Test Spell Push Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

            /*Fire Properties
             * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
             * */
            float baseDmg = 10f;
            GameObject obj = Ray_.Data.collider.gameObject;
            ForceObject(obj, -baseDmg);
        }),
        Colors = ColourDict[Elements.Push]
    };


    SpellEffector PullPlayer = new SpellEffector() {
        DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Pull",
        Effector = new Action<EffectorData>((EffectorData_) => {
            RayData Ray_ = (RayData)EffectorData_;
            Debug.Log($"Test Spell Pull Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

            /*Fire Properties
             * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
             * */
            float baseDmg = 10f;
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            ForceObject(obj, baseDmg);
        }),
        Colors = ColourDict[Elements.Pull]
    };

    SpellEffector PushPlayer = new SpellEffector() {
        DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Push",
        Effector = new Action<EffectorData>((EffectorData_) => {
            RayData Ray_ = (RayData)EffectorData_;
            Debug.Log($"Test Spell Push Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

            /*Fire Properties
             * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
             * */
            float baseDmg = 10f;
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            ForceObject(obj, -baseDmg);

        }),
        Colors = ColourDict[Elements.Push]
    };

    static void ForceObject(GameObject obj, float baseDmg) {
        float commonFactor = 20f;
        float force = baseDmg * commonFactor;
        Vector2 facing = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
        Debug.Log(obj.transform.name + " was hit.");
        if (obj.TryGetComponent<EntityManager>(out EntityManager otherEntity)) {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (otherEntity.entityProperties.Contains(Properties.Light)) {
                rb.AddForce(1.5f * force * facing);
            } else if (otherEntity.entityProperties.Contains(Properties.Heavy)) {
                rb.AddForce(0.4f * force * facing);
            } else if (!otherEntity.entityProperties.Contains(Properties.Immovable)) {
                rb.AddForce(force * facing);
            }
        }
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Pull);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Push);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(PullPlayer);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(PushPlayer);
    }
}


