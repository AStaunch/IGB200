using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EnumsAndDictionaries;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(AbstractDoor))]
public class DoorEditorScript : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
    void OnSceneGUI() {
        AbstractDoor abstractDoor = target as AbstractDoor;
        Handles.color = Color.yellow;
        if (abstractDoor.ExitDoor != null) {
            Handles.DrawLine(abstractDoor.transform.position, abstractDoor.ExitDoor.transform.position);
        }
    }
}
