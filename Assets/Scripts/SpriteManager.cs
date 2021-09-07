using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [System.Serializable]
    public struct SpriteCollection
    {
        string name;
        Sprite[] Sprites;
    }


    public SpriteCollection[] SpriteCollections;

    private static SpriteManager _instance;

    public static SpriteManager Instance { get { return _instance; } }


    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
}
