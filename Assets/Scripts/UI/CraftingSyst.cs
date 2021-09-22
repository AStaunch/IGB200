using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSyst : MonoBehaviour
{
    public HotbarHandler hotbarscript;

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
        if (effect == null || template == null)
            Debug.Log($"Spell Build Failed: effect present: {effect != null}, template present: {template != null}");
        else
            StartCoroutine("BuildSet");
    }

    public void ResetUI()
    {
        UI_EffectorDisplay.sprite = null;
        UI_EffectorDisplay.color = Color.clear;
        UI_TemplateDisplay.sprite = null;
        UI_TemplateDisplay.color = Color.clear;
    }

    IEnumerator BuildSet()
    {
        Debug.Log("Select Hotbar slot to place spell");
        bool KeyChosen = false;
        while (!KeyChosen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 0);
                effect = null;
                template = null;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 1);
                effect = null;
                template = null;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 2);
                effect = null;
                template = null;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 3);
                effect = null;
                template = null;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 4);
                effect = null;
                template = null;
                ResetUI();
                KeyChosen = true;
            }

            yield return null;
        }
        StopCoroutine("BuildSet");
        yield return null;
    }
}
