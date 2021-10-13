using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class CustomEventHandle : MonoBehaviour
{
    #region crafting Evnt syst
    #region custom evnt handling
    public class EventData
    {
        public enum EvntType
        {
            Template,
            Effect
        }

        public string eventName;
        public GameObject SentBy;
        public Sprite SlotSprite;
        public SpellEffector effector;
        public SpellTemplate template;
        public EvntType type;
    }

    public class EvntHndl_args
    {
        public EvntHndl_args() { }
        public EventData eventData;
    }

    public delegate void CustomEventHandler(object sender, EvntHndl_args e);
    public event CustomEventHandler onBroadcast;
    #endregion

    [Header("Crafting system Event items")]
    public EventData.EvntType dataType;
    public string eventName;
    public string effector_name;
    public string template_name;
    public Sprite Crafting_Slot_img;


    public void BroadcastEvent()
    {

        EventData eventData = new EventData()
        {
            SentBy = gameObject,
            type = dataType,
            eventName = eventName,
            SlotSprite = Crafting_Slot_img//gameObject.GetComponent<Image>().sprite
        };

        if (dataType == EventData.EvntType.Effect)
            eventData.effector = Effectors.Find(effector_name);
        if (dataType == EventData.EvntType.Template)
            eventData.template = SpellRegistrySing.Instance.Registry.QueryRegistry(template_name);


        onBroadcast.Invoke(this, new EvntHndl_args() { eventData = eventData });
    }
    #endregion

    #region Unlock syst
    
    public Button Mine;
    [Header("Loot system shit")]
    public bool Disabled = true;

    private void Start()
    {
        if (gameObject.GetComponent<Button>() && Mine == null)
        {
            Mine = gameObject.GetComponent<Button>();
        }

        //register the template or effect  - THIS IS NOW HANDLED ELSE WHERE
        //if(!Array.Exists(UnlockManager.Instance.Registry.AllKeys(), (i) => { return i == effector_name ? true : i == template_name ? true : false; })){
        //    UnlockManager.Instance.Registry.AddUnlockItem(new SpellWrapper(dataType == EventData.EvntType.Template ? UnlockType.TEMPLATE : UnlockType.EFFECTOR, dataType == EventData.EvntType.Template ? SpellRegistrySing.Instance.Registry.QueryRegistry(template_name) : null, dataType == EventData.EvntType.Effect ? Effectors.Find(effector_name) : null));
        //}

        //UnlockManager.Instance.Registry.ItemUnlocked += Registry_ItemUnlocked;

        UpdateImg();
    }

    public void Registry_ItemUnlocked(UnlockArgs args)
    {
        if(dataType == EventData.EvntType.Template && args.Item.Type() == UnlockType.TEMPLATE)
        {
            if (args.name == template_name)
            {
                Disabled = false;
                UpdateImg();
            }
        }
        else if (dataType == EventData.EvntType.Effect && args.Item.Type() == UnlockType.EFFECTOR)
        {
            if (args.name == effector_name)
            {
                Disabled = false;
                UpdateImg();
            }
        }
    }

    private void UpdateImg()
    {
        Mine.interactable = !Disabled;
    }

    #endregion
}
