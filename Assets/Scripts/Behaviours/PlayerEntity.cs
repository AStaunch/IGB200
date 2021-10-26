using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;

public class PlayerEntity : AbstractCreature
{
    #region Singleton Things
    private static PlayerEntity _instance;
    public static PlayerEntity Instance { get { return _instance; } }
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion
    public AbstractDoor LastDoor_ { get => LastDoor; set => LastDoor = value; }

    public override bool IsEnemy => false;

    private AbstractDoor LastDoor;
    List<Rigidbody2D> collidedObjects = new List<Rigidbody2D>();
    private Collider2D CollisionCollider;
    public RoomData StartRoomData_ {get => StartRoomData; set => StartRoomData = value; }
    private RoomData StartRoomData;
    private Vector3 StartPosition_;
    private Vector3 change;
    // Update is called once per frame
    bool NoClip;

    private void Start() {
        StartPosition_ = transform.position;
        DefaultMat = GetComponent<SpriteRenderer>().material;
        Deceleration_ = 5;
        Health_ = MaxHealth_;
        EntitySpeed_ = 5;
        gameObject.layer = 7;
        if (DamageImmunities_ == null) {
            DamageImmunities_ = new Elements[0];
        }

        Collider2D[] AllColliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in AllColliders) {
            if (!collider.isTrigger) {
                CollisionCollider = collider;
                NoClip = collider.isTrigger;
                break;
            }
        }

    }
    void Update()
    {
        if(this.gameObject.layer == 7) {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            if (change != Vector3.zero) {
                if (VectorToDirection(change) == GetEntityDirectionEnum()) {
                    UpdateVelocity(EntitySpeed_, change.normalized);
                }
                UpdateDirection(change);
                UpdateAnimation(RB_.velocity);
            } else {
                UpdateAnimation(RB_.velocity);
            }
        }
        //Restart Room
        if (Input.GetKeyDown(KeyCode.R)) {
            EntityDeath();
        }
        //Turns of Player Collision
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P)) {
            NoClip = !NoClip;
            CollisionCollider.isTrigger = NoClip;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftBracket)) {
            string[] UnlockNames = UnlockManager.Instance.Registry.AllKeys();
            foreach (string UnlockName in UnlockNames) {
                if (Array.Exists(UnlockManager.Instance.Registry.AllKeys(), (e) => { return e == UnlockName; })) {
                    UnlockManager.Instance.Registry.UnlockItem(UnlockName);
                    GameObject.FindGameObjectWithTag("TextBox").GetComponent<DebugBox>().inputs.Add("Spell.unlock(" + UnlockName + ");");
                } else {
                    throw new Exception($"Item by the name {UnlockName} does not exist within the unlock manager");
                }
            }
        }
    }
    public void CastSpell(SpellTemplate CallingSpell)
    {
        RB_.velocity = Vector2.zero;
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("attack");
        string CallingSpellName = CallingSpell.Name;
        if (CallingSpellName.Contains("Arc"))
        {
            CallingSpellName = "Arc";
        }
        if (CallingSpellName.Contains("Orb")) { Instantiate(SoundManager.SoundDict["OrbThrowSound"]); }
        else { Instantiate(SoundManager.SoundDict[CallingSpellName + "Sound"]); }
        if (CallingSpellName.Contains("Ray")) { GameObject.FindGameObjectWithTag("TextBox").GetComponent<DebugBox>().inputs.Add("Player.raycast(parameter);"); }
        if (CallingSpellName.Contains("Arc")) { GameObject.FindGameObjectWithTag("TextBox").GetComponent<DebugBox>().inputs.Add("Player.arc(parameter):"); }
        if (CallingSpellName.Contains("Cone")) { GameObject.FindGameObjectWithTag("TextBox").GetComponent<DebugBox>().inputs.Add("Object.coneCast(parameter);"); }
        if (CallingSpellName.Contains("Orb")) { GameObject.FindGameObjectWithTag("TextBox").GetComponent<DebugBox>().inputs.Add("Object.instantiateOrb(parameter);"); }
    }
    public override void EntityDeath() {
        Health_ = MaxHealth_;
        transform.position = StartPosition_;
        StartRoomData_.Load();
        if (LastDoor != null) {
            LastDoor.RoomData_.Unload();
        }
    }
    protected override void EntityFall() {
        if (LastDoor != null) {
            TakeDamage(1f, Elements.NULL);
            transform.position = LastDoor_.RespawnPoint;
            LastDoor.RoomData_.Load();
        }
    }

    public override void UpdateAnimation(Vector3 change) {
        if (change != Vector3.zero) {
            Anim_.SetFloat("moveX", change.x);
            Anim_.SetFloat("moveY", change.y);
            Anim_.SetBool("moving", true);
        } else {
            Anim_.SetBool("moving", false);
        }
    }

    public override void UpdateVelocity(float magnitude, Vector3 direction) {
            RB_.velocity = magnitude * direction;
    }
    public override void Decelerate() {
        if (RB_.velocity != Vector2.zero && change == Vector3.zero && gameObject.layer == 7) {
            RB_.velocity *= 0.1f;
        }
    }


    //Hacky Checkpoint Management

    //Very Hacky Kino Management
    private void OnCollisionEnter2D(Collision2D collision) {
        bool b1 = collision.transform.TryGetComponent(out iPropertyInterface _) && !collision.transform.TryGetComponent(out EmptySpaceScript _); 
        bool b2 = collision.transform.TryGetComponent(out Rigidbody2D rb);
        if (b1 && b2) {
            collidedObjects.Add(rb);
            rb.isKinematic = true;
            rb.useFullKinematicContacts = true;
            rb.velocity = Vector2.zero;
        }
        EnterVoid(collision);
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out Rigidbody2D rb)){
            if(collidedObjects.Contains(rb)) {
                collidedObjects.Remove(rb);
                rb.isKinematic = false;
            }
        }
        LeaveVoid(collision);
    }

}
