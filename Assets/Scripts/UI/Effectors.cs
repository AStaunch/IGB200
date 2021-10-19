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
                
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            otherEntity.TakeDamage(EffectorData_.baseStrength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcOther(ac, EffectorData_.baseStrength, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_,EffectorData_.baseStrength, element);
                        break;
                                            
                    case "Orb":
                        OrbData Orb_ = (OrbData)EffectorData_;
                        OrbScript orbScript = Orb_.Data.AddComponent<OrbScript>();
                        orbScript.element = element;
                        orbScript.baseDamage = EffectorData_.baseStrength;
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
                Elements element = Elements.Pull;
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        Vector2 direction = Ray_.CasterObject.transform.GetComponent<iFacingInterface>().GetEntityDirection();
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iPhysicsInterface otherEntity)) {
                            otherEntity.UpdateForce(EffectorData_.baseStrength,direction, element);
                            Ray_.Data.collider.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(CheckVelocityCanBridgeGaps(Ray_.Data.collider.gameObject));
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcOther(ac, EffectorData_.baseStrength, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, EffectorData_.baseStrength, element);
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
                
                Elements element = Elements.Pull;
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        Ray_.CasterObject.GetComponent<MonoBehaviour>().StartCoroutine(LerpSelf(Ray_.CasterObject, Ray_.Data.point, 1f));
                        break;
                                            
                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcPlayer(ac, Arc_, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_, EffectorData_.baseStrength, element);
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
                
                Elements element = Elements.Push;
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        Vector2 direction = Ray_.CasterObject.transform.GetComponent<iFacingInterface>().GetEntityDirection();
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iPhysicsInterface otherEntity)) {
                            otherEntity.UpdateForce(EffectorData_.baseStrength, direction, element);
                            Ray_.Data.collider.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(CheckVelocityCanBridgeGaps(Ray_.Data.collider.gameObject));
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcOther(ac, EffectorData_.baseStrength, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_,EffectorData_.baseStrength, element);
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
                Elements element = Elements.Push;
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;                        
                        if (Ray_.CasterObject.TryGetComponent(out iPhysicsInterface thisEntity)) {
                            Vector2 Direction = Ray_.CasterObject.transform.GetComponent<iFacingInterface>().GetEntityDirection();
                            thisEntity.UpdateForce(EffectorData_.baseStrength, Direction, element);
                            EffectorData_.CasterObject.GetComponent<MonoBehaviour>().StartCoroutine(CheckVelocityCanBridgeGaps(EffectorData_.CasterObject));
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcPlayer(ac, Arc_, element));                        
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        float TotalForce = 0;
                        foreach (GameObject gameObject in Cone_.Data) {
                            if (gameObject.TryGetComponent(out iPhysicsInterface HI)) {
                                Vector2 Direction = gameObject.transform.position - Cone_.CasterObject.transform.position;
                                HI.UpdateForce(EffectorData_.baseStrength, Direction, element);
                                gameObject.GetComponent<MonoBehaviour>().StartCoroutine(CheckVelocityCanBridgeGaps(gameObject));
                                Debug.DrawLine(Cone_.CasterObject.transform.position, Cone_.CasterObject.transform.position, Color.magenta, 1f);
                            }
                            TotalForce += Vector2.Distance(Cone_.CasterObject.transform.position, gameObject.transform.position);
                        }
                        Vector2 direction = Cone_.CasterObject.transform.GetComponent<iFacingInterface>().GetEntityDirection();
                        Cone_.CasterObject.GetComponent<iPhysicsInterface>().UpdateForce(TotalForce, direction, element);
                        Cone_.CasterObject.GetComponent<MonoBehaviour>().StartCoroutine(CheckVelocityCanBridgeGaps(Cone_.CasterObject));
                        Debug.Log(TotalForce * direction);
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
                
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                        case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            otherEntity.TakeDamage(EffectorData_.baseStrength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcOther(ac, EffectorData_.baseStrength, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_,EffectorData_.baseStrength, element);
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Ice");
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
                
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            otherEntity.TakeDamage(EffectorData_.baseStrength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcOther(ac, EffectorData_.baseStrength, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_,EffectorData_.baseStrength, element);
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Life");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Life]
        },
        #endregion
        #region Electricity
        new SpellEffector() {
            Name = "Electricity",
            Effector = new Action<iEffectorData>((EffectorData_) => {
                Elements element = Elements.Electricity;
                
                string CallingTemp = EffectorData_.Calling_template.Name;
                if (CallingTemp.Contains("Arc")) {
                    CallingTemp = "Arc";
                }
                switch (CallingTemp) {
                    case "Ray":
                        RayData Ray_ = (RayData)EffectorData_;
                        if (Ray_.Data.collider.gameObject.TryGetComponent(out iHealthInterface otherEntity)) {
                            otherEntity.TakeDamage(EffectorData_.baseStrength, element);
                        }
                        break;

                    case "Arc":
                        ArcData Arc_ = (ArcData)EffectorData_;
                        ArcScript ac = Arc_.Data.AddComponent<ArcScript>();
                        ac.direction = Arc_.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
                        ac.arcDirection = Arc_.ArcDirection;
                        ac.GetComponent<MonoBehaviour>().StartCoroutine(ArcOther(ac, EffectorData_.baseStrength, element));
                        break;

                    case "Cone":
                        ConeData Cone_ = (ConeData)EffectorData_;
                        ConeProcess(Cone_,EffectorData_.baseStrength, element);
                        break;

                    default:
                        Debug.Log($"{EffectorData_.Calling_template.Name} is not yet defined for Electricity");
                        break;
                }
            }),
            Colors = ColourDict[Elements.Electricity]
        },
        #endregion
    };


    public static SpellEffector Find(string name) {
        return SpellEffects.Find((e) => { return e.Name == name; });
    }
}
