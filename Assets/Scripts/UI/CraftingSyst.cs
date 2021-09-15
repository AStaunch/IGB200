using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSyst : MonoBehaviour
{

    HotbarItem[] Hotbar = new HotbarItem[5];
    public class HotbarItem
    {
        public SpellEffector effector;
        public SpellTemplate template;
        public void run()
        {
            template.RunFunction(effector);
        }
    }


    public bool Self_Populate_Events = false;


    public Image UI_TemplateDisplay;
    public Image UI_EffectorDisplay;


    private SpellEffector effect;
    private SpellTemplate template;

    [SerializeField]
    List<CustomEventHandle> Events = new List<CustomEventHandle>();

    private void Awake()
    {
        if (Self_Populate_Events)
        {
            Events.Clear();
            Events.AddRange(gameObject.GetComponentsInChildren<CustomEventHandle>(false));
        }

        foreach(CustomEventHandle customEvent in Events)
        {
            customEvent.onBroadcast += onRecieved;
        }
    }

    private void onRecieved(object sender, CustomEventHandle.EvntHndl_args e)
    {
        Debug.Log(e.eventData.eventName);
        switch (e.eventData.type)
        {
            case CustomEventHandle.EventData.EvntType.Effect:
                effect = e.eventData.effector;

                UI_EffectorDisplay.sprite = e.eventData.SentBy_Sprite;
                UI_EffectorDisplay.color = Color.white;
                break;


            case CustomEventHandle.EventData.EvntType.Template:
                template = e.eventData.template;

                UI_TemplateDisplay.sprite = e.eventData.SentBy_Sprite;
                UI_TemplateDisplay.color = Color.white;
                break;
        }
    }





    public void BuildSpell()
    {
        if(effect == null || template == null)
        {
            Debug.Log($"Spell Build Failed: effect present: {effect != null}, template present: {template != null}");
        }
        else
        {
            Hotbar[0] = new HotbarItem() { effector = effect, template = template };

            effect = null;
            template = null;
            ResetUI();
        }
    }

    public void ResetUI()
    {
        UI_EffectorDisplay.sprite = null;
        UI_EffectorDisplay.color = Color.clear;
        UI_TemplateDisplay.sprite = null;
        UI_TemplateDisplay.color = Color.clear;
    }
}
