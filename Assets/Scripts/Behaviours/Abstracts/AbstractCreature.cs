using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SoundManager;
using static SpellFunctionLibrary;
public abstract class AbstractCreature : MonoBehaviour, iHealthInterface, iCreatureInterface, iPhysicsInterface, iPropertyManager, iFacingInterface
{
    public abstract bool IsEnemy { get; }
    public int Health_ { get => Health; set => Health = value; }
    private int Health;
    public int MaxHealth_ { get => MaxHealth; set => MaxHealth = value; }
    public int MaxHealth = 1;
    public Elements[] DamageImmunities_ { get => DamageImmunities; set => DamageImmunities = value; }
    public Elements[] DamageImmunities = new Elements[0];

    public Properties[] EntityProperties_ { get => EntityProperties; set => EntityProperties = value; }
    [SerializeField]
    private Properties[] EntityProperties = new Properties[0];
    public EntityTypes EntityType_ { get => EntityType; set => EntityType = value; }
    private EntityTypes EntityType = EntityTypes.Creature;
    public float EntitySpeed_ { get => Speed; set => Speed = value; }
    [SerializeField]
    private float Speed = 0.5f;
    public Animator Anim_ { get => GetComponent<Animator>();}
    public Directions CurrentDirection_ { get => CurrentDirection; set => CurrentDirection = value; }
    private Directions CurrentDirection;
    public Rigidbody2D RB_ { get => GetComponent<Rigidbody2D>();}
    public float Deceleration_ { get => Deceleration; set => Deceleration = value; }
    private float Deceleration = 1;
    private Collider2D PhysicsCollider {
        get {
            Collider2D[] AllColliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in AllColliders) {
                if (!collider.isTrigger) {
                    return collider;
                }
            }
            return GetComponent<Collider2D>();
        }
    }
    protected Material DefaultMat;

    private void FixedUpdate() {
        Decelerate();
        UpdateSorting();
    }

    private void Awake() {
        GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
    }
    public Vector2 GetEntityDirection() {
        return VectorDict[CurrentDirection_];
    }

    public Directions GetEntityDirectionEnum() {
        return CurrentDirection_;
    }

    public int GetEntityFacing() {
        return IntDict[CurrentDirection_];
    }
    IEnumerator SpriteRoutine = null;

    public void TakeDamage(float damage, Elements damageType, SpellTemplates damageSource = SpellTemplates.NULL) {
        damage = ComputeSpellStrength(damageType, EntityProperties_, damage);
        int damageInt = Mathf.RoundToInt(damage);
        string SoundName = damageType.ToString() + "Damage";
        //Check if this does damage
        if (DamageImmunities_.Contains(damageType) || damageInt == 0) {
            SoundName = "AttackFail";
            Instantiate(SoundDict[SoundName]);
            return;
        }
        //Instantiate Damage Sound        
        Instantiate(SoundDict[SoundName]);
        //Create Damage Effect
        SpellRenderer hitDrawer = FindObjectOfType<SpellRenderer>();
        hitDrawer.CreateBurstFX(transform.position, ColourDict[damageType]);
        // Discolour Sprite Freeze
        if(SpriteRoutine != null) { StopCoroutine(SpriteRoutine); }

        if (damageType.Equals(Elements.Ice)) {
            Debug.Log($"{transform.name} is frozen!");
            SpriteRoutine = TintSprite(2.5f, Color.cyan);
            StartCoroutine(SpriteRoutine);
            StartCoroutine(MovePause(2.5f));
        } else {
            if (damageInt > 0) {
                Debug.Log($"{transform.name} takes {damageInt} {damageType} damage!");
                SpriteRoutine =  TintSprite(0.1f, Color.red);
                StartCoroutine(SpriteRoutine);
            } else if (damageInt < 0) {
                Debug.Log($"{transform.name} is healed for {damageInt} points!");
                SpriteRoutine = TintSprite(0.1f, Color.green);
                StartCoroutine(SpriteRoutine);
            }
            //Process Health
            Health_ -= damageInt;
            Health_ = Mathf.Clamp(Health_, 0, MaxHealth_);
        }
        //Check if the entity should die
        if (0 >= Health_) {
            Debug.Log($"{transform.name} dies!!!");
            Anim_.SetTrigger("death");
            GameObject.FindGameObjectWithTag("TextBox").GetComponent<DebugBox>().inputs.Add("Object.Destroy(collision.enemy);");
        }
    }

    public abstract void UpdateAnimation(Vector3 change);

    public void UpdateDirection(Vector3 change) {
        if (change != Vector3.zero) {
            CurrentDirection_ = VectorToDirection(change);
        }
    }
    public void UpdateSorting() {
        if (PhysicsCollider) {
            GetComponent<Renderer>().sortingOrder = -Mathf.RoundToInt(PhysicsCollider.bounds.center.y);
        } else {
            GetComponent<Renderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y);
        }

    }

    public abstract void  UpdateVelocity(float magnitude, Vector3 direction);
    public void UpdateForce(float magnitude, Vector3 direction, Elements elementType) {
        magnitude = ComputeSpellStrength(elementType, EntityProperties_, magnitude);
        if (EntityProperties_.Contains(Properties.Immovable)) {
            return;
        }
        if (elementType == Elements.Pull) {
            magnitude *= -1f;
        }
        gameObject.layer = 6;
        Instantiate(SoundDict[elementType.ToString() + "Damage"]);
        RB_.AddForce(magnitude * RB_.mass * direction, ForceMode2D.Impulse);
        Debug.Log(magnitude * RB_.mass * direction);
    }



    public void AlertObservers(AnimationEvents message) {
        if (message.Equals(AnimationEvents.Death)) {
            EntityDeath();
        }else if (message.Equals(AnimationEvents.Fall)){
            EntityFall();
        }
    }
    protected void CheckFalling(Collision2D collision) {
        if (collision.collider.TryGetComponent(out EmptySpaceScript _) && !collision.collider.isTrigger) {
            //Debug.Log(collision.contactCount);
            bool Falling = false;
            Vector2 offset = 0.1f * PhysicsCollider.bounds.size;
            float offsetX = offset.x; float offsetY = offset.y;

            //float offset = 0.1f * PhysicsCollider.bounds.size;
            //float offsetX = offset; float offsetY = offset;
            foreach (ContactPoint2D contact in collision.contacts) {
                bool XBounds = contact.point.x > PhysicsCollider.bounds.min.x + offsetX && contact.point.x < PhysicsCollider.bounds.max.x - offsetX;
                bool YBounds = contact.point.y > PhysicsCollider.bounds.min.y + offsetY && contact.point.y < PhysicsCollider.bounds.max.y - offsetY;
                //Debug.Log(XBounds + ":" + YBounds);
                if (XBounds && YBounds) {
                    Falling = true;
                } else {
                    return;
                }
            }
            if (Falling) {
                Anim_.SetTrigger("fall");
            }
        } else {
            return;
        }
    }
    protected abstract void EntityFall();
    public abstract void Decelerate();
    public abstract void EntityDeath();

    #region Property Management
    public void AddProperty(Properties property) {
        if (!EntityProperties_.Contains(property)) {
            //Add Property
            Array.Resize(ref EntityProperties, EntityProperties_.Length + 1);
            EntityProperties_[EntityProperties_.Length - 1] = property;
        }
    }

    public void RemovePropery(Properties property) {
        if (EntityProperties_.Contains(property)) {
            int index = Array.FindIndex(EntityProperties, 0, EntityProperties_.Length, EntityProperties_.Contains);
            for (; index < EntityProperties_.Length - 1; index++) {
                EntityProperties_[index] = EntityProperties_[index + 1];
            }
            Array.Resize(ref EntityProperties, EntityProperties_.Length - 1);
        }
    }

    public void AddProperty(Properties property, float duration) {
        if (!EntityProperties_.Contains(property)) {
            StartCoroutine(AddPropertyForDuration(property, duration));
        }
    }

    private IEnumerator AddPropertyForDuration(Properties property, float duration) {
        float t = 0;
        Array.Resize(ref EntityProperties, EntityProperties_.Length + 1);
        EntityProperties_[EntityProperties_.Length - 1] = property;

        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        Array.Resize(ref EntityProperties, EntityProperties_.Length - 1);

    }


    public void AddImmunity(Elements element) {
        if (!DamageImmunities_.Contains(element)) {
            //Add Property
            Array.Resize(ref DamageImmunities, DamageImmunities_.Length + 1);
            DamageImmunities_[DamageImmunities_.Length - 1] = element;
        }
    }

    public void RemoveImmunity(Elements property) {
        if (DamageImmunities_.Contains(property)) {
            int index = Array.FindIndex(DamageImmunities, 0, DamageImmunities_.Length, DamageImmunities_.Contains);
            for (; index < DamageImmunities_.Length - 1; index++) {
                DamageImmunities_[index] = DamageImmunities_[index + 1];
            }
            Array.Resize(ref EntityProperties, EntityProperties_.Length - 1);
        }
    }

    public void AddImmunity(Elements element, float duration) {
        if (!DamageImmunities_.Contains(element)) {
            StartCoroutine(AddImmunityForDuration(element, duration));
        }
    }

    private IEnumerator AddImmunityForDuration(Elements element, float duration) {
        float t = 0;
        Array.Resize(ref DamageImmunities, DamageImmunities_.Length + 1);
        DamageImmunities_[DamageImmunities_.Length - 1] = element;

        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        Array.Resize(ref DamageImmunities, DamageImmunities_.Length - 1);
    }
    public IEnumerator MovePause(float Wait) {
        float SpeedStore = EntitySpeed_;
        float time = 0;
        while (time < Wait) {
            EntitySpeed_ = 0f;
            time += Time.deltaTime;
            yield return null;
        }
        EntitySpeed_ = SpeedStore;
    }
    public IEnumerator TintSprite(float duration, Color color) {
        float time = 0;
        byte Alpha = 192;
        GetComponent<SpriteRenderer>().material = SpriteManager.Instance.CreateTint(color, Alpha);
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }
        GetComponent<SpriteRenderer>().material = SpellRenderer.Instance.defaultUnlit;
    }

    private float ComputeSpellStrength(Elements element, Properties[] properties, float strength) {
        if (properties.Length == 0) {
            return strength;
        }
        if (properties.Contains(ElementPropertyPairs[element][0])) {
            strength *= ElementValuePairs[element][0];
        } else if (properties.Contains(ElementPropertyPairs[element][1])) {
            strength *= ElementValuePairs[element][1];
        }
        if (properties.Contains(Properties.Immovable) && (element == Elements.Pull || element == Elements.Push)) {
            strength = 0f;
        }
        return strength;
    }
    #endregion
}