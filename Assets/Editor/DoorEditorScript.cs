//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using static EnumsAndDictionaries;
//using UnityEditor.AnimatedValues;

//[CustomEditor(typeof(DoorEntity))]
//public class DoorEditorScript : Editor
//{
//    AnimBool isSceneDoor;

//    private void OnEnable() {
//        isSceneDoor = new AnimBool(false);
//        isSceneDoor.valueChanged.AddListener(Repaint);
//    }

//    private void OnGUI() {
//        GUILayout.Label("Door Settings", EditorStyles.boldLabel);
//    }

//    public override void OnInspectorGUI() {
//        //EditorGUI.BeginChangeCheck();
//        DoorEntity myDoor = (DoorEntity)target;


//        myDoor.exitDirection = (Directions)EditorGUILayout.EnumPopup("Door Facing", myDoor.exitDirection);
//        isSceneDoor.target = EditorGUILayout.ToggleLeft("Scene Door?", isSceneDoor.target);
//        myDoor.isOpen = EditorGUILayout.ToggleLeft("Is Open?", myDoor.isOpen);
//        myDoor.isOpen = EditorGUILayout.ToggleLeft("Is Fireproof?", myDoor.isFireproof);

//        EditorGUI.indentLevel++;
//        if (EditorGUILayout.BeginFadeGroup(isSceneDoor.faded)) {
//            GUILayout.Label("Scene Index of Destination");
//            myDoor.sceneIndex = EditorGUILayout.IntSlider(myDoor.sceneIndex, 0, UnityEngine.SceneManagement.SceneManager.sceneCount);
//        } else {
//            myDoor.ExitDoor = (DoorEntity)EditorGUILayout.ObjectField("Connected Door", myDoor.ExitDoor, typeof(DoorEntity), true);
//        }
//        EditorGUI.indentLevel--;
//        EditorGUILayout.EndFadeGroup();
//        EditorGUILayout.Space();

//        //if (EditorGUI.EndChangeCheck()) {
//        //    myDoor.OnValidate();
//        //}
//    }
//}
