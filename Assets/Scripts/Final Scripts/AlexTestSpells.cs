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
        //DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Fire",
        Effector = new Action<EffectorData>((EffectorData_) => {
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":
                    RayData Ray_ = (RayData)EffectorData_;
                    //Debug.Log($"Test Spell Fire recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

                    /*Fire Properties
                     * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                     * */
                    float baseDmg = 10f;
                    //Debug.Log(Ray_.Data.collider.transform.name + " was hit.");
                    if (Ray_.Data.collider.gameObject.TryGetComponent<EntityManager>(out EntityManager otherEntity)) {
                        if (otherEntity.entityProperties.Contains(Properties.Flamable)) {
                            otherEntity.TakeDamage(baseDmg * 2f);
                        } else if (otherEntity.entityProperties.Contains(Properties.Fireproof)) {
                            otherEntity.TakeDamage(0f);
                        } else {
                            otherEntity.TakeDamage(baseDmg);
                        }
                    }
                    break;

                case "Arc":
                    ArcData Arc_ = (ArcData)EffectorData_;

                    
                    break;
                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Fire");
                    break;

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
        //DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Pull",
        Effector = new Action<EffectorData>((EffectorData_) => {
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":
                    RayData Ray_ = (RayData)EffectorData_;
                //Debug.Log($"Test Spell Pull Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

                /*Fire Properties
                 * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                 * */
                float baseDmg = 10f;
                GameObject obj = Ray_.Data.collider.gameObject;
                ForceObject(obj, baseDmg);
                    break;
                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Pull");
                    break;
                }
        }),
        Colors = ColourDict[Elements.Pull]

    };

    SpellEffector Push = new SpellEffector() {
        //DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Push",
        Effector = new Action<EffectorData>((EffectorData_) => {
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":
                    RayData Ray_ = (RayData)EffectorData_;
                //Debug.Log($"Test Spell Push Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");
                /*Fire Properties
                    * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                    * */
                float baseDmg = 10f;
                GameObject obj = Ray_.Data.collider.gameObject;
                ForceObject(obj, -baseDmg);
                    break;
                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Pull");
                    break;
            }
        }),
        Colors = ColourDict[Elements.Push]
    };

    


    SpellEffector PullPlayer = new SpellEffector() {
        //DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Pull",
        Effector = new Action<EffectorData>((EffectorData_) => {
        RayData Ray_ = (RayData)EffectorData_;
        //Debug.Log($"Test Spell Pull Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");
        /*Fire Properties
         * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
         * */
        float baseDmg = 10f;
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        MonoBehaviour mono = obj.GetComponent<MonoBehaviour>();
        mono.StartCoroutine(LerpObject(obj, Ray_.Data.point, 1f));
        }),
        Colors = ColourDict[Elements.Pull]
    };

    SpellEffector PushPlayer = new SpellEffector() {
        //DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
        Name = "Push",
        Effector = new Action<EffectorData>((EffectorData_) => {
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                    Debug.Log($"Test Spell Push Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

                    /*Fire Properties
                     * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                     * */
                    float baseDmg = 10f;
                    GameObject obj = GameObject.FindGameObjectWithTag("Player");
                    ForcePlayerRay(obj, -baseDmg);
                    break;

                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for PushPlayer");
                    break;
            }
        }),
        Colors = ColourDict[Elements.Push]
    };
    private float timer;



    static void ForceObject(GameObject obj, float baseDmg) {
        float commonFactor = 0.25f;
        float force = baseDmg * commonFactor;
        Vector2 facing = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
        if (obj.TryGetComponent(out EntityManager em)) {
            Vector2 forceVector = force * facing;
            if (em.entityProperties.Contains(Properties.Light)) {
                forceVector *= 1.5f;
            } else if (em.entityProperties.Contains(Properties.Heavy)) {
                forceVector *= 0.5f;
            } else if (em.entityProperties.Contains(Properties.Immovable)) {
                forceVector *= 0f;
            }
            em.UpdateVelocity(forceVector);
        }
    }

    static void ForcePlayerBack(GameObject obj, Vector2 direction, float baseDmg) {

    }


    static IEnumerator LerpObject(GameObject gameObject, Vector2 targetPosition, float duration) {
        EntityManager em = gameObject.GetComponent<EntityManager>();
        Rigidbody2D rb = gameObject.transform.GetComponent<Rigidbody2D>();

        float time = 0;
        Vector2 startPosition = gameObject.transform.position;
        targetPosition -= 0.6f * gameObject.GetComponent<SpriteRenderer>().bounds.size * em.GetEntityDirection();
        while (time < duration) {
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            time += Time.deltaTime;
            gameObject.layer = 6;
            yield return null;
        }
        rb.MovePosition(targetPosition);
        gameObject.layer = 7;
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
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                UpdateTimer();
                SpellRegistrySing.Instance.Registry.QueryRegistry("Arc").RunFunction(Fire);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
               UpdateTimer();
               SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(PullPlayer);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6)) {
               UpdateTimer();
               SpellRegistrySing.Instance.Registry.QueryRegistry("Ray").RunFunction(PushPlayer);
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    void UpdateTimer() {
        timer = Time.timeSinceLevelLoad + 1f;
    }
}


