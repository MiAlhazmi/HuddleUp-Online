using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour {

    [HideInInspector]
    public GameObject LampLight;

    [HideInInspector]
    public GameObject DomeOff;

    [HideInInspector]
    public GameObject DomeOn;

    public bool turnOn = false;

    private LampInteract _lampInteract;
    
    

	// Use this for initialization
	void Start ()
	{
		_lampInteract = GetComponent<LampInteract>();
	}

    public void OnButtonClicked()
    {
	    turnOn = !turnOn;
	    if (turnOn)
	    {
		    LampLight.SetActive(true);
		    DomeOff.SetActive(false);
		    DomeOn.SetActive(true);
		    _lampInteract.promptMessage = "Turn lamp off";
	    }
	    else
	    {
		    LampLight.SetActive(false);
		    DomeOff.SetActive(true);
		    DomeOn.SetActive(false);
		    _lampInteract.promptMessage = "Turn lamp on";
	    }
    }
}
