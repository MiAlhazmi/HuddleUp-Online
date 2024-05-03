using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // message displayed to player when looking at an interactable.
    public string promptMessage;

    // this function will be called by our player
    public void BaseInteract(GameObject playerGameObj)
    {
        Interact(playerGameObj);
    }
    
    protected virtual void Interact(GameObject playerGameObj)
    {
        // we wont have any code written in this function
        // this is just a template function to be overridden by or subclasses.
    }
}
