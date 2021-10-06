using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpellRenderer;

public class SwitchObject : MonoBehaviour, iSenderObject, iHealthInterface
{
    private Material[] materials;
    public bool currentState;
    public bool currentState_ {
        get {
            return currentState;
        }
        set {
            currentState = value;            
            UpdateReciever();
        }
    }
    private List<iRecieverObject> targetObjects = new List<iRecieverObject>();
    public List<iRecieverObject> targetObjects_ { get => targetObjects; set => targetObjects = value; }

    public Elements element;

    private void UpdateReciever() {
        UpdateSprite();
        foreach (iRecieverObject target in targetObjects_) {
            target.CheckSenders(this);
        }
    }
    // Start is called before the first frame update
    private void Start() {
        Health_ = MaxHealth_;
        EntityProperties_ = new Properties[] { Properties.Immovable };
        currentState_ = false;
        shader = FindObjectOfType<SpellRenderer>().shader;
        materials = new Material[] { CreateMaterial(ColourDict[Elements.NULL], shader), CreateMaterial(ColourDict[element], shader) };
        UpdateSprite();    
    }
    private void UpdateSprite() {
        if (!currentState_) {
            GetComponent<SpriteRenderer>().material = materials[1];
        } else {
            GetComponent<SpriteRenderer>().material = materials[0];
        }
    }


    public Properties[] EntityProperties_ { get => EntityProperties; set => EntityProperties = value; }
    private Properties[] EntityProperties;
    public EntityTypes EntityType_ { get => EntityType; set => EntityType = value; }
    private EntityTypes EntityType;
    public Directions CurrentDirection_ { get => CurrentDirection; set => CurrentDirection = value; }
    private Directions CurrentDirection;
    public int Health_ { get => Health; set => Health = value; }
    private int Health;
    public int MaxHealth_ { get => MaxHealth; set => MaxHealth = value; }
    private int MaxHealth = 1;
    public Elements[] DamageImmunities_ { get => null; set => _ = value; }


    private Shader shader;

    public void TakeDamage(float damage, Elements damageType) {
        if(damageType != element) {
            return;
        }
        Health_ -= Mathf.RoundToInt(damage);
        Health_ = Mathf.Clamp(Health_, 0, MaxHealth_);
        if (0 >= Health_) {
            EntityDeath();
        }
    }

    public void EntityDeath() {
        currentState_ = true;
        UpdateSprite();
    }

    private void OnValidate() {
        shader = FindObjectOfType<SpellRenderer>().shader;
        materials = new Material[] { CreateMaterial(ColourDict[Elements.NULL], shader), CreateMaterial(ColourDict[element], shader) };
        UpdateSprite();
    }
}
