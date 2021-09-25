using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface iRecieverObject
    {
        public Dictionary<iRecieverObject, bool> Switches { get; set; }
        public bool checkRecievedSignals();
    }

    public interface iSenderObjects
    {

        public iSenderObjects[] switchTriggers { get; set; }
        public void addObject();
}


