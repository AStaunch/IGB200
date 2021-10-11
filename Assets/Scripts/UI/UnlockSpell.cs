using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockSpell : MonoBehaviour
{
    public string UnlockName = "";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Array.Exists(UnlockManager.Instance.Registry.AllKeys(), (e) => { return e == UnlockName; }))
        {
            UnlockManager.Instance.Registry.UnlockItem(UnlockName);
        }
        else
        {
            throw new Exception($"Item by the name {UnlockName} does not exist within the unlock manager");
        }
    }
}
