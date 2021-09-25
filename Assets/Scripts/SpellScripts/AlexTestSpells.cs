using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpellFunctionLibrary;


public class AlexTestSpells : MonoBehaviour
{  

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
                    GameObject obj = Ray_.Data.collider.gameObject;
                    Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
                    if (obj.transform.GetComponent<EntityManager>().entityProperties.Contains(Properties.Immovable)){
                        baseStrength = 0f;
                    }
                    ForceObject(obj, direction, baseStrength);
                    obj.GetComponent<MonoBehaviour>().StartCoroutine(LerpObject(obj, Ray_.Data.point, 1f));
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
                    GameObject obj = Ray_.Data.collider.gameObject;
                    Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
                    ForceObject(obj, direction, -baseStrength);
                    if (obj.transform.GetComponent<EntityManager>().entityProperties.Contains(Properties.Immovable)){
                        baseStrength = 0f;
                    }
                    obj.GetComponent<MonoBehaviour>().StartCoroutine(LerpObject(obj, Ray_.Data.point, 1f));
                    break;

                default:
                    Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Pull");
                    break;
            }
        }),
        Colors = ColourDict[Elements.Push]
    };

    


    SpellEffector PullPlayer = new SpellEffector() {
        Name = "PullPlayer",
        Effector = new Action<EffectorData>((EffectorData_) => {

            RayData Ray_ = (RayData)EffectorData_;

            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            MonoBehaviour mono = obj.GetComponent<MonoBehaviour>();
            mono.StartCoroutine(LerpObject(obj, Ray_.Data.point, 1f));
        }),
        Colors = ColourDict[Elements.Pull]
    };

    SpellEffector PushPlayer = new SpellEffector() {
        Name = "PushPlayer",
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



    
    
    private void Start() {
        timer = Time.timeSinceLevelLoad;
    }

    
}


