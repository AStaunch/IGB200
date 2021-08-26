using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;


public class RegistryLiveMenu : EditorWindow
{
    private static int selected = 0;
    [MenuItem("SpellReg/Registry Live Menu")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(RegistryLiveMenu)).Show();
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            List<GUIContent> uIContent = new List<GUIContent>();
            foreach (SpellTemplate spltmp in SpellRegistrySing.Instance.Registry.QueryAll())
            {
                uIContent.Add(new GUIContent(spltmp.Name));
            }


            selected = EditorGUI.Popup(new Rect(0, 0, this.position.width, 20), selected, uIContent.ToArray());
            EditorGUILayout.Space();
            GUILayout.BeginArea(new Rect(0, 25, this.position.width, 50));
            GUILayout.Button("");
            GUILayout.EndArea();
        }
    }
}
