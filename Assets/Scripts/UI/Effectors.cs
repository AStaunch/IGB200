using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Effectors
{
    public static List<SpellEffector> SpellEffects = new List<SpellEffector>()
    {
        new SpellEffector()
        {
            Name = "Test",
            Colors = new Color[] { Color.white, Color.white, Color.white, Color.white },
            Effector = new Action<EffectorData>((edd) => { Debug.Log("works"); })
        }
    };

    public static SpellEffector Find(string name)
    {
        return SpellEffects.Find((e) => { return e.Name == name; });
    }
}
