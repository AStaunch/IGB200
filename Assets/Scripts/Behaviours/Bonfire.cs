using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public class Bonfire : MonoBehaviour, iDamageInterface, iSenderObject, iReloadInterface
{
    public List<iRecieverObject> targetObjects_ { get => targetObjects; set => targetObjects = value; }
    private List<iRecieverObject> targetObjects = new List<iRecieverObject>();
    public bool currentState_ { get => currentState; set { currentState = value; UpdateReciever(); } }

    public bool isException_ => isException;
    public bool isException = true;
    private Animator Anim_ => GetComponent<Animator>();
    private bool currentState = false;
    public Vector3 spawnLocation;
    public Directions spawnDirection = Directions.Down;
    RoomData spawnRoom;
    public void Start() {
        spawnLocation = transform.position + (Vector3) VectorDict[spawnDirection];
    }
    public void EntityDeath() {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage, Elements damageType, SpellTemplates damageSource = SpellTemplates.NULL) {
        if(damageType == Elements.Fire) {
            currentState_ = true;
            return;
        }
        //if (damageType == Elements.Earth || damageType == Elements.Ice) {
        //    currentState_ = false;
        //    return;
        //}

    }

    public void UpdateReciever() {
        foreach (iRecieverObject target in targetObjects_) {
            target.CheckSenders(this);
        }
        if (currentState_) {
            SetPlayerCheckpoint();
        }        
    }

    private void SetPlayerCheckpoint() {
        Bonfire[] bonfires = FindObjectsOfType<Bonfire>();
        foreach(Bonfire bonfire in bonfires) {
            if (bonfire == this) {
                Anim_.SetTrigger("light");
                PlayerEntity.Instance.SavePosition_ = this.spawnLocation;
            } else {
                bonfire.currentState_ = false;
                bonfire.Anim_.SetTrigger("dark");
            }
        }

    }

    public void ResetSender() {
        throw new System.NotImplementedException();
    }
}
