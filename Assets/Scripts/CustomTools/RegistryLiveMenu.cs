using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using System;


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


            selected = EditorGUI.Popup(new Rect(0, 0, this.position.width, (float)(this.position.height * 0.05)), selected, uIContent.ToArray());
            EditorGUILayout.Space();
            GUILayout.BeginArea(new Rect(0, (float)(this.position.height * 0.05) + 1, this.position.width, (float)(this.position.height * 0.05)));
            //EditorGUI.DrawPreviewTexture(new Rect(0, (float)(this.position.height * 0.05) * 2 + 2, (float)(this.position.width / 0.75), (float)(this.position.height * 0.05)), SpellRegistrySing.Instance.Registry.QueryRegistry(uIContent[selected].text).icon.texture);
            if (GUILayout.Button("Test fire"))
                SpellRegistrySing.Instance.Registry.QueryRegistry(uIContent[selected].text).RunFunction(new SpellEffector()
                {
                    Name = "TestFire",
                    FireEffect = new Action<GameObject>((gmeobj) =>
                    {
                        Debug.Log($"--Test fire invoked--");
                    })
                });
            GUILayout.EndArea();
        }
    }
}
