using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Dialogue
{
    public string name = "Computer";
    [TextArea(3, 10)]
    public string[] sentences;
    public Sprite CharacterPortrait;
}
