using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorInteract : Interactable
{
    // [SerializeField] private Color color;
    [SerializeField] private MeshRenderer _meshRenderer;
    private Color _thisColor;
    private bool _locked;
    private GameObject _colorOwner; // this points to the player who took the color
    
    // Start is called before the first frame update
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _thisColor = _meshRenderer.material.color;
        _locked = false;
    }

    protected override void Interact(GameObject playerGameObj)
    {
        if (!_locked || _colorOwner.GetComponent<Player>().GetPlayerColor() != _thisColor)
        {
            playerGameObj.GetComponent<Player>().ChangePlayerColor(_meshRenderer.material.color);
            _colorOwner = playerGameObj;
            _locked = true;
        }
    }
}
