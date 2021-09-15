using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomEventHandle : MonoBehaviour
{
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
        public Sprite SentBy_Sprite;
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

    public EventData.EvntType dataType;
    public string eventName;
    public string effector_name;
    public string template_name;

    public void BroadcastEvent()
    {

        EventData eventData = new EventData()
        {
            SentBy = gameObject,
            type = dataType,
            eventName = eventName,
            SentBy_Sprite = gameObject.GetComponent<Image>().sprite
        };

        if (dataType == EventData.EvntType.Effect)
            eventData.effector = Effectors.Find(effector_name);
        if (dataType == EventData.EvntType.Template)
            eventData.template = SpellRegistrySing.Instance.Registry.QueryRegistry(template_name);


        onBroadcast.Invoke(this, new EvntHndl_args() { eventData = eventData });
    }

}
