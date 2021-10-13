using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpriteManager;

public class ChestScript : MonoBehaviour, iSenderObject
{
    public bool currentState_ { get { return currentState; } set { currentState = value; UpdateSprite(value); } }
    private bool currentState;   
    public List<iRecieverObject> targetObjects_ { get => targetObjects; set => targetObjects = value; }
    private List<iRecieverObject> targetObjects = new List<iRecieverObject>();

    private void UpdateReciever() {
        foreach (iRecieverObject target in targetObjects_) {
            target.CheckSenders(this);
        }
    }

    private void UpdateSprite(bool value) {
        if (!value) {
            GetComponent<SpriteRenderer>().sprite = SpriteDict["ChestSprites"][0];
        } else {
            GetComponent<SpriteRenderer>().sprite = SpriteDict["ChestSprites"][1];
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && currentState) {
            currentState_ = false;
        }
    }
}