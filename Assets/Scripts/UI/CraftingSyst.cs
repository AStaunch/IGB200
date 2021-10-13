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

    public Text Hotbar_msg;


    private SpellEffector effect;
    private SpellTemplate template;

    [SerializeField]
    List<CustomEventHandle> Events = new List<CustomEventHandle>();

    private void Start()
    {
        if (Self_Populate_Events)
        {
            //Events.Clear();
            Events.AddRange(gameObject.GetComponentsInChildren<CustomEventHandle>(false));
        }

        foreach(CustomEventHandle customEvent in Events)
        {
            customEvent.onBroadcast += onRecieved;
            UnlockManager.Instance.Registry.ItemUnlocked += customEvent.Registry_ItemUnlocked;


            SpellWrapper spw = new SpellWrapper(UnlockType.TEMPLATE, null, null);
            if(customEvent.dataType == CustomEventHandle.EventData.EvntType.Template)
            {
                SpellTemplate template = SpellRegistrySing.Instance.Registry.QueryRegistry(customEvent.template_name);
                spw = new SpellWrapper(UnlockType.TEMPLATE, template, null);
            }
            else
            {
                SpellEffector effector = Effectors.Find(customEvent.effector_name);
                spw = new SpellWrapper(UnlockType.EFFECTOR, null, effector);
            }
            UnlockManager.Instance.Registry.AddUnlockItem(spw);
        }
    }

    private void onRecieved(object sender, CustomEventHandle.EvntHndl_args e)
    {
        Debug.Log(e.eventData.eventName);
        switch (e.eventData.type)
        {
            case CustomEventHandle.EventData.EvntType.Effect:
                effect = e.eventData.effector;

                UI_EffectorDisplay.sprite = e.eventData.SlotSprite;
                UI_EffectorDisplay.color = Color.white;
                break;


            case CustomEventHandle.EventData.EvntType.Template:
                template = e.eventData.template;

                UI_TemplateDisplay.sprite = e.eventData.SlotSprite;
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
        Hotbar_msg.gameObject.SetActive(true);
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
        Hotbar_msg.gameObject.SetActive(false);
        yield return null;
    }
}
