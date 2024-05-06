using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectInteract : Interactable
{
    [SerializeField] private MapSelectionControl _mapSelectionControl;
    private ObjectSelect _objectSelect;

    private void Awake()
    {
        _objectSelect = GetComponent<ObjectSelect>();
    }

    protected override void Interact(GameObject playerGameObj)
    {
        _mapSelectionControl.SelectMap(intInput);
        // gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        _objectSelect.Select();
    }
}
