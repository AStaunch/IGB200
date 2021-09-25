using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EnumsAndDictionaries;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(NormalDoor))]
public class DoorEditorScript : Editor{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
    void OnSceneGUI() {
        NormalDoor normalDoor = target as NormalDoor;
        Handles.color = Color.yellow;
        if (normalDoor.ExitDoor != null) {
            Handles.DrawLine(normalDoor.transform.position, normalDoor.ExitDoor.transform.position);
        }
    }
}
