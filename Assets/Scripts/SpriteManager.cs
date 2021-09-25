using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public SpriteCollection4Way[] DirectionalSpriteCollections;
    public SpriteCollection[] SpriteCollections;
    public static Dictionary<string, Sprite[]> SpriteDict;
    private static SpriteManager _instance;
    public static SpriteManager Instance { get { return _instance; } }

    [ExecuteInEditMode]
    private void Awake() {

        StoreVariables();

        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void OnValidate() {
        StoreVariables();
    }

    private void StoreVariables() {
        SpriteDict = new Dictionary<string, Sprite[]>();
        foreach (SpriteCollection4Way collection in DirectionalSpriteCollections) {
            SpriteDict.Add(collection.name, collection.Sprites);
        }
        foreach (SpriteCollection collection in SpriteCollections) {
            SpriteDict.Add(collection.name, collection.Sprites);
        }
    }
}

[System.Serializable]
public struct SpriteCollection4Way
{
    public string name; 
    public Sprite Up;
    public Sprite Down;
    public Sprite Left;
    public Sprite Right;
    public Sprite[] Sprites => new Sprite[] { Up, Right, Down, Left };
}

[System.Serializable]
public struct SpriteCollection
{
    public string name;
    public Sprite[] Sprites;
}
