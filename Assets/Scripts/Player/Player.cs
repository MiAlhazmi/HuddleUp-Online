using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // private MeshRenderer _meshRenderer;
    
    private PlayerUI _playerUI;
    [SerializeField] private GameObject _visual; // visual gameobject
    [SerializeField] private Color _playerColor; // This attribute is for the color that the player chooses
    private SkinnedMeshRenderer _skinnedMesh;
    [SerializeField] private bool isTagger = false;



    private void Awake()
    {
        _playerUI = GetComponent<PlayerUI>();
        if (_visual == null) GameObject.FindWithTag("Visual");
        else _skinnedMesh = _visual.GetComponent<SkinnedMeshRenderer>();
        _playerColor = Color.blue;
        ChangeColor(_playerColor);
    }

    public void DeActivatePlayer()
    {
        _visual.SetActive(false);
        gameObject.GetComponent<PlayerHit>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<CharacterController>().enabled = false;
    }
    
    public void ActivatePlayer()
    {
        _visual.SetActive(true);
        gameObject.GetComponent<PlayerHit>().enabled = true;
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        gameObject.GetComponent<CharacterController>().enabled = true;
        SetIsTagger(false);
    }
    
    public void SetIsTagger(bool paraIsTagger)
    {
        isTagger = paraIsTagger;
        if (isTagger) ChangeColor(Color.red);
        else ChangeColor(_playerColor);
        
        _playerUI.ShowTagOverlay(isTagger); // to show the effect on screen indicating if he's the tagger or not
    }

    public bool GetIsTagger()
    {
        return isTagger;
    }

    private void ChangeColor(Color color)
    {
        // _meshRenderer.material.color = color;
        _skinnedMesh.material.color = color;
    }

    // This function is for the color that the player chooses
    public void ChangePlayerColor(Color color)
    {
        _playerColor = color;
        ChangeColor(_playerColor);
    }

    public Color GetPlayerColor()
    {
        return _playerColor;
    }
}
