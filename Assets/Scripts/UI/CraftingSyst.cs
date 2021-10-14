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
    public Image UI_AdditionalParam;

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


            //SpellWrapper spw = new SpellWrapper(UnlockType.TEMPLATE, null, null);
            if(customEvent.dataType == CustomEventHandle.EventData.EvntType.Template)
            {
                SpellTemplate template = SpellRegistrySing.Instance.Registry.QueryRegistry(customEvent.template_name);
                SpellWrapper spw = new SpellWrapper(UnlockType.TEMPLATE, template, null);
                UnlockManager.Instance.Registry.AddUnlockItem(spw);
                if (customEvent.Disabled == false)
                    UnlockManager.Instance.Registry.UnlockItem(spw.GetValue().Template.Name);
            }
            else
            {
                SpellEffector effector = Effectors.Find(customEvent.effector_name);
                SpellWrapper spw = new SpellWrapper(UnlockType.EFFECTOR, null, effector);
                UnlockManager.Instance.Registry.AddUnlockItem(spw);
                if (customEvent.Disabled == false)
                    UnlockManager.Instance.Registry.UnlockItem(spw.GetValue().Effector.Name);
            }
        }
    }


    private bool AdditionalParam = false;
    private GameObject ActiveAdditionalMenu;

    private void onRecieved(object sender, CustomEventHandle.EvntHndl_args e)
    {
        //only effects have additional params since this is Old El Paso cheating and the "params" are actually coded, and we just shift depending on this instead and ignore the base one
        if (e.eventData.type == CustomEventHandle.EventData.EvntType.Effect && e.eventData.HasAdditionalParam)
        {
            AdditionalParam = e.eventData.HasAdditionalParam;
            ActiveAdditionalMenu = e.eventData.AdditionalParam_menu;
            ActiveAdditionalMenu.SetActive(true);

            UI_EffectorDisplay.sprite = e.eventData.SlotSprite;
            UI_EffectorDisplay.color = Color.white;
        }
        else if(AdditionalParam && e.eventData.IsAdditionalParam)
        {
            effect = e.eventData.effector;

            UI_AdditionalParam.sprite = e.eventData.SlotSprite;
            UI_AdditionalParam.color = Color.white;
        }
        else
        {
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
        UI_AdditionalParam.sprite = null;
        UI_AdditionalParam.color = Color.clear;
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
                ActiveAdditionalMenu.SetActive(false);
                ActiveAdditionalMenu = null;
                AdditionalParam = false;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 1);
                effect = null;
                template = null;
                ActiveAdditionalMenu.SetActive(false);
                ActiveAdditionalMenu = null;
                AdditionalParam = false;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 2);
                effect = null;
                template = null;
                ActiveAdditionalMenu.SetActive(false);
                ActiveAdditionalMenu = null;
                AdditionalParam = false;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 3);
                effect = null;
                template = null;
                ActiveAdditionalMenu.SetActive(false);
                ActiveAdditionalMenu = null;
                AdditionalParam = false;
                ResetUI();
                KeyChosen = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                hotbarscript.AssignSpell(new HotbarHandler.HotbarItem() { effector = effect, template = template }, 4);
                effect = null;
                template = null;
                ActiveAdditionalMenu.SetActive(false);
                ActiveAdditionalMenu = null;
                AdditionalParam = false;
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
