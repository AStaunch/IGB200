using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;
using static EnumsAndDictionaries;

public class AlexTestSpells : MonoBehaviour
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
    private float timer;

    static void ForceObject(GameObject obj, float baseDmg) {
        float commonFactor = 40f;
        float force = baseDmg * commonFactor;
        Vector2 facing = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
        Debug.Log(obj.transform.name + " was hit.");
        if (obj.TryGetComponent(out EntityManager otherEntity) && obj.TryGetComponent(out Rigidbody2D rb)) {
            if (otherEntity.entityProperties.Contains(Properties.Light)) {
                rb.velocity = (1.5f * force * facing);
            } else if (otherEntity.entityProperties.Contains(Properties.Heavy)) {
                rb.velocity = (0.4f * force * facing);
            } else if (!otherEntity.entityProperties.Contains(Properties.Immovable)) {
                rb.AddForce(force * facing);
            } else {
                rb.velocity = force * facing;
            }
        }
    }
    private void Awake() {
        timer = Time.timeSinceLevelLoad;
    }

    void Update() {

        if (timer < Time.timeSinceLevelLoad){
            if ( Input.GetKeyDown(KeyCode.Alpha1)){
                UpdateTimer();
                SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Fire);
            }
            if ( Input.GetKeyDown(KeyCode.Alpha2)){
                UpdateTimer();
                SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Pull);
            }
            if ( Input.GetKeyDown(KeyCode.Alpha3)){
                UpdateTimer();
                SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(Push);
            }
            //if ( Input.GetKeyDown(KeyCode.Alpha4)){
            //    UpdateTimer();
            //    SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(PullPlayer);
            //}
            //if ( Input.GetKeyDown(KeyCode.Alpha5)) {
            //    UpdateTimer();
            //    SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(PushPlayer);
            //}
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    void UpdateTimer() {
        timer = Time.timeSinceLevelLoad + 1f;
    }
}


