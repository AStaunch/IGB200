using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface iRecieverObject
    {
    //public Dictionary<iSenderObjects, bool> Switches { get; set; }
}

public interface iSenderObjects
    {

        public iSenderObjects[] switchTriggers { get; set; }
        public void addObject();
}


