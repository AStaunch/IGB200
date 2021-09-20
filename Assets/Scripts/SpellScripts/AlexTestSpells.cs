using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;
using static EnumsAndDictionaries;

public class AlexTestSpells : MonoBehaviour
{
    private static Dictionary<Elements, Properties[]> ElementPropertyPairs = new Dictionary<Elements, Properties[]> {
        {Elements.Fire, new Properties[] {Properties.Flamable, Properties.Fireproof} },
        {Elements.Pull, new Properties[] {Properties.Light, Properties.Heavy} },
        {Elements.Push, new Properties[] {Properties.Light, Properties.Heavy} },
    };

    private static Dictionary<Elements, float[]> ElementValuePairs = new Dictionary<Elements, float[]> {
        {Elements.Fire, new float[] {2f, 0f} },
        {Elements.Ice, new float[]  {2f, 0f} },
        {Elements.Pull, new float[] {1.5f, 0.5f} },
        {Elements.Push, new float[] {1.5f, 0.5f} },
    };

    static float ComputeOutPutValue(Elements element, EntityManager otherEntity, float inputValue) {

        if (otherEntity.entityProperties.Contains(ElementPropertyPairs[element][0])) {
            inputValue *= ElementValuePairs[element][0];
        } else if (otherEntity.entityProperties.Contains(ElementPropertyPairs[element][1])) {
            inputValue *= ElementValuePairs[element][1];
        }

        if (otherEntity.entityProperties.Contains(Properties.Immovable) && (element == Elements.Pull || element == Elements.Push)) {
            inputValue = 0f;
        }
        return inputValue;
    }
    SpellEffector Fire = new SpellEffector() {
        Name = "Fire",
        Effector = new Action<EffectorData>((EffectorData_) => {
            float baseStrength = EffectorData_.baseStrength;
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":
                    RayData Ray_ = (RayData)EffectorData_;
                    /*Fire Properties
                     * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                     * */
                    //Debug.Log(Ray_.Data.collider.transform.name + " was hit.");
                    if (Ray_.Data.collider.gameObject.TryGetComponent(out EntityManager otherEntity)) {
                        float Strength = ComputeOutPutValue(Elements.Fire, otherEntity, baseStrength);
                        otherEntity.TakeDamage(Strength);
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
        Colors = ColourDict[Elements.Fire]
    };

    SpellEffector Pull = new SpellEffector() {
        Name = "Pull",
        Effector = new Action<EffectorData>((EffectorData_) => {
            float baseStrength = EffectorData_.baseStrength;
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":
                    RayData Ray_ = (RayData)EffectorData_;
                    //Debug.Log($"Test Spell Pull Ray recieved: {Ray_.Data.point.x} - {Ray_.Data.point.y}");

                    /*Fire Properties
                        * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                        * */
                    GameObject obj = Ray_.Data.collider.gameObject;
                    Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
                    ForceObject(obj, direction, baseStrength);
                    break;

                case "Arc":
                    ArcData Arc_ = (ArcData)EffectorData_;
                    GameObject ArcObject = Arc_.Data;
                    break;

                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Pull");
                    break;
                }
        }),
        Colors = ColourDict[Elements.Pull]
    };

    SpellEffector Push = new SpellEffector() {
        Name = "Push",
        Effector = new Action<EffectorData>((EffectorData_) => {
            float baseStrength = EffectorData_.baseStrength;
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":
                    RayData Ray_ = (RayData)EffectorData_;
                
                    /*Fire Properties
                    * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                    * */
                    GameObject obj = Ray_.Data.collider.gameObject;
                    Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
                    ForceObject(obj, direction, -baseStrength);
                    break;
                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Pull");
                    break;
            }
        }),
        Colors = ColourDict[Elements.Push]
    };

    


    SpellEffector PullPlayer = new SpellEffector() {
        Name = "Pull",
        Effector = new Action<EffectorData>((EffectorData_) => {

            RayData Ray_ = (RayData)EffectorData_;

            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            MonoBehaviour mono = obj.GetComponent<MonoBehaviour>();
            mono.StartCoroutine(LerpObject(obj, Ray_.Data.point, 1f));
        }),
        Colors = ColourDict[Elements.Pull]
    };

    SpellEffector PushPlayer = new SpellEffector() {
        Name = "Push",
        Effector = new Action<EffectorData>((EffectorData_) => {
            float baseStrength = EffectorData_.baseStrength;
            switch (EffectorData_.Calling_template.Name) {
                case "Ray":

                    /*Fire Properties
                     * Damage to enemies is based on their properties. this code could be recycled for other pieces, changing base damage among other things.
                     * */
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    Vector2 direction = playerObj.transform.GetComponent<EntityManager>().GetEntityDirection();
                    baseStrength *= playerObj.transform.GetComponent<EntityManager>().Deceleration;
                    ForceObject(playerObj, direction, -baseStrength);
                    MonoBehaviour mono = playerObj.GetComponent<MonoBehaviour>();
                    mono.StartCoroutine(CheckVelocityCanBridgeGaps(playerObj));
                    break;

                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for PushPlayer");
                    break;
            }
        }),
        Colors = ColourDict[Elements.Push]
    };
    private float timer;



    static void ForceObject(GameObject obj, Vector2 direction, float baseStrength) {
        float commonFactor = 0.25f;
        if (obj.TryGetComponent(out EntityManager em)) {
            baseStrength = ComputeOutPutValue(Elements.Push, em, baseStrength);
            float force = baseStrength * commonFactor;
            Vector2 forceVector = force * direction;
            em.UpdateVelocity(forceVector);
        }
    }

    static IEnumerator CheckVelocityCanBridgeGaps(GameObject gameObject) {
        Rigidbody2D rb = gameObject.transform.GetComponent<Rigidbody2D>();

        while (rb.velocity.magnitude > 1f) {
            gameObject.layer = 6;
            yield return null;
        }
        gameObject.layer = 7;
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
    
    private void Start() {
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


