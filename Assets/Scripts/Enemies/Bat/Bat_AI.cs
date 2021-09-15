using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_AI : MonoBehaviour
{
    #region Notes

    //States are as follows
    //---------------------
    // ‣ Stationary
    // ‣ Tracking
    // ‣ Chasing

    //Tracking
    //<===================>
    //Tracking is where the bat has no line of sight on the player, or the player has dropped out of visual range
    //if this happens, the bat will go to the last known point
    //pretty basic shit

    //Chasing
    //<===================>
    //litterally tracking but the player is in view, so the bat just goes for it
    #endregion


    Coroutine Active_State;


    void Start()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    public IEnumerator Tracking()
    {
        Vector2 knownPos;

        yield return new WaitForEndOfFrame();
    }

    private void OnDrawGizmos()
    {
        
    }

    

}
