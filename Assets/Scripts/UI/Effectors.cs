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
            Effector = new Action<iEffectorData>((edd) => { Debug.Log("works"); })
        },
        # region Fire
        new SpellEffector() {
            Name = "Fire",
            Effector = new Action<iEffectorData>((EffectorData_) => {
                Elements element = Elements.Fire;
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            float Strength = ComputeOutPutValue(element, otherEntity.EntityProperties_, baseStrength);
                            otherEntity.TakeDamage(Strength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcBehaviour ac = Arc_.Data.AddComponent<ArcBehaviour>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcHitDetection(ac, Arc_, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, element);
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
            Effector = new Action<iEffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                Elements element = Elements.Pull;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        GameObject obj = Ray_.Data.collider.gameObject;
                        Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<iCreatureInterface>().GetEntityDirection();
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iPhysicsInterface otherEntity)) {
                            float Strength = ComputeOutPutValue(Elements.Pull,otherEntity.EntityProperties_ , baseStrength);
                            otherEntity.UpdateVelocity(Strength, direction.normalized);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        GameObject ArcObject = Arc_.Data;
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, element);
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
            Effector = new Action<iEffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        player.GetComponent<MonoBehaviour>().StartCoroutine(LerpSelf(player, Ray_.Data.point, 1f));
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
            Effector = new Action<iEffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                Elements element = Elements.Push;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        GameObject obj = Ray_.Data.collider.gameObject;
                        Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<iCreatureInterface>().GetEntityDirection();
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iPhysicsInterface otherEntity)) {
                            float Strength = ComputeOutPutValue(Elements.Push, otherEntity.EntityProperties_, baseStrength);
                            otherEntity.UpdateVelocity(Strength, direction);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcBehaviour ac = Arc_.Data.AddComponent<ArcBehaviour>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcHitDetection(ac, Arc_, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, element);
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
            Effector = new Action<iEffectorData>((EffectorData_) => {
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        GameObject obj = GameObject.FindGameObjectWithTag("Player");
                        
                        Vector2 direction = obj.GetComponent<iCreatureInterface>().GetEntityDirection();
                        float currentStrength = baseStrength * 2;
                        currentStrength = ComputeOutPutValue(Elements.Push, obj.transform.GetComponent<iPhysicsInterface>().EntityProperties_ , currentStrength);
                        obj.transform.GetComponent<iPhysicsInterface>().UpdateVelocity(-currentStrength,direction);
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
            Effector = new Action<iEffectorData>((EffectorData_) => {
                Elements element = Elements.Ice;
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                        case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            float Strength = ComputeOutPutValue(element, otherEntity.EntityProperties_, baseStrength);
                            otherEntity.TakeDamage(Strength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcBehaviour ac = Arc_.Data.AddComponent<ArcBehaviour>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcHitDetection(ac, Arc_, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, element);
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Fire");
                        break;                
                }
            }),
            Colors = ColourDict[Elements.Ice]
        },
        #endregion
        #region Life
        new SpellEffector() {
            Name = "Life",
            Effector = new Action<iEffectorData>((EffectorData_) => {
                Elements element = Elements.Life;
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            float Strength = ComputeOutPutValue(element, otherEntity.EntityProperties_, baseStrength);
                            otherEntity.TakeDamage(-Strength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcBehaviour ac = Arc_.Data.AddComponent<ArcBehaviour>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcHitDetection(ac, Arc_, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, element);
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Fire");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Ice]
        },
        #endregion
        #region Electricity
        new SpellEffector() {
            Name = "Electricity",
            Effector = new Action<iEffectorData>((EffectorData_) => {
                Elements element = Elements.Electricity;
                float baseStrength = EffectorData_.baseStrength;
                switch (EffectorData_.Calling_template.Name) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            float Strength = ComputeOutPutValue(element, otherEntity.EntityProperties_, baseStrength);
                            otherEntity.TakeDamage(Strength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcBehaviour ac = Arc_.Data.AddComponent<ArcBehaviour>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcHitDetection(ac, Arc_, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, element);
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Fire");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Ice]
        },
        #endregion

    };

    private static void ConeProcess(ConeData Cone_, Elements element) {
        foreach (GameObject gameObject in Cone_.Data) {
            if(element == Elements.Pull || element == Elements.Push) {
                if (gameObject.TryGetComponent(out iPhysicsInterface HI)) {
                    float Strength = ComputeOutPutValue(element, HI.EntityProperties_, Cone_.baseStrength);
                    Vector2 Direction = gameObject.transform.position - Cone_.CasterObject.transform.position;
                    HI.UpdateVelocity(Strength, Direction);
                    Debug.DrawLine(Cone_.CasterObject.transform.position, Cone_.CasterObject.transform.position, Color.magenta, 1f);
                }
            } else {
                if (gameObject.TryGetComponent(out iHealthInterface HI)) {
                    float Strength = ComputeOutPutValue(element, HI.EntityProperties_, Cone_.baseStrength);
                    HI.TakeDamage(Strength, element);
                    Debug.DrawLine(Cone_.CasterObject.transform.position, Cone_.CasterObject.transform.position, Color.red, 1f);
                }
            }
        }
    }

    public static SpellEffector Find(string name) {
        return SpellEffects.Find((e) => { return e.Name == name; });
    }
}
