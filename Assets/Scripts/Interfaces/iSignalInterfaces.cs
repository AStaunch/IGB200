using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iRecieverObject
{
    public iSenderObject[] switchObjects_ { get; set; }
    bool currentState_ { get; set; }
    public void CheckSenders();
}

public interface iSenderObject
{
    public List<iRecieverObject> targetObjects_ { get; set; }
    public bool currentState_ { get; set; }
}