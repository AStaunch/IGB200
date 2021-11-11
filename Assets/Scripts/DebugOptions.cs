using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOptions : MonoBehaviour
{
    internal Collider2D CollisionCollider;
    internal bool NoClip;
    // Start is called before the first frame update
    void Start()
    {
        Collider2D[] AllColliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in AllColliders) {
            if (!collider.isTrigger) {
                CollisionCollider = collider;
                NoClip = collider.isTrigger;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Loads main game
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Slash)) {
            MainMenuScript.LOADSCENE(MainMenuScript.gameID);
        }
        //Turns of Player Collision
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P)) {
            NoClip = !NoClip;
            CollisionCollider.isTrigger = NoClip;
        }
        //Teleport player to cursor
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T)) {
            Vector3 newpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newpos.z = 0;
            PlayerEntity.Instance.transform.position = newpos;
        }
        //Unlocks all spells
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftBracket)) {
            string[] UnlockNames = UnlockManager.Instance.Registry.AllKeys();
            foreach (string UnlockName in UnlockNames) {
                if (Array.Exists(UnlockManager.Instance.Registry.AllKeys(), (e) => { return e == UnlockName; })) {
                    UnlockManager.Instance.Registry.UnlockItem(UnlockName);
                    DebugBox.Instance.inputs.Add("Spell.unlock(" + UnlockName + ");");
                } else {
                    throw new Exception($"Item by the name {UnlockName} does not exist within the unlock manager");
                }
            }
        }
        //Restart Room
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
            PlayerEntity.Instance.EntityFall();
            PlayerEntity.Instance.Health_ += 1;
        }
    }
}
