using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpellFunctionLibrary;

public static class Effectors
{
    public static List<SpellEffector> SpellEffects = new List<SpellEffector>()
    {
        new SpellEffector()
        {
            Name = "Test",
            Colors = new Color[] { Color.white, Color.white, Color.white, Color.white },
            Effector = new Action<EffectorData>((edd) => { Debug.Log("works"); })
        },
# region Fire
        new SpellEffector() {
            Name = "Fire",
            Effector = new Action<EffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
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
        },
        #endregion
        #region Pull
        new SpellEffector() {
            Name = "Pull",
            Effector = new Action<EffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        GameObject obj = Ray_.Data.collider.gameObject;
                        Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out EntityManager otherEntity)) {
                            float Strength = ComputeOutPutValue(Elements.Pull, otherEntity, baseStrength);
                            ForceObject(obj, direction, -Strength);
                        }
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
        },
        new SpellEffector() {
            Name = "PullPlayer",
            Effector = new Action<EffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        GameObject obj = GameObject.FindGameObjectWithTag("Player");
                        obj.GetComponent<MonoBehaviour>().StartCoroutine(LerpObject(obj, Ray_.Data.point, 1f));
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for PullPlayer");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Pull]
    },
        #endregion
        #region Push
        new SpellEffector() {
            Name = "Push",
            Effector = new Action<EffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        GameObject obj = Ray_.Data.collider.gameObject;
                        Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();

                        if (Ray_.Data.collider.gameObject.TryGetComponent(out EntityManager otherEntity)) {
                            float Strength = ComputeOutPutValue(Elements.Push, otherEntity, baseStrength);
                            ForceObject(obj, direction, Strength);                            
                        }
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Push");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Push]
        },

        new SpellEffector() {
            Name = "PushPlayer",
            Effector = new Action<EffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        GameObject obj = GameObject.FindGameObjectWithTag("Player");
                        EntityManager em =  obj.transform.GetComponent<EntityManager>();
                        Vector2 direction = em.GetEntityDirection();
                        float Strength = baseStrength * em.Deceleration  * 2;
                        Strength = ComputeOutPutValue(Elements.Push, em, Strength);
                        ForceObject(obj, direction, -Strength);
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for PushPlayer");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Push]
    },
        #endregion
        #region Ice
        new SpellEffector() {
            Name = "Ice",
            Effector = new Action<EffectorData>((EffectorData_) => {
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;

                        GameObject obj = GameObject.FindGameObjectWithTag("Player");
                        MonoBehaviour mono = obj.GetComponent<MonoBehaviour>();
                        mono.StartCoroutine(LerpObject(obj, Ray_.Data.point, 1f));
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for PullPlayer");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Pull]
    },
};

    public static SpellEffector Find(string name) {
        return SpellEffects.Find((e) => { return e.Name == name; });
    }
}
