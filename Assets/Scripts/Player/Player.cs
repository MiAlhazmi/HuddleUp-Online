using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // private MeshRenderer _meshRenderer;
    
    [SerializeField] private bool isTagger = false;


    private void Awake()
    {
        // _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetIsTagger(bool paraIsTagger)
    {
        isTagger = paraIsTagger;
        if (isTagger) ChangeColor(Color.red);
        else ChangeColor(Color.white);
    }

    public bool GetIsTagger()
    {
        return isTagger;
    }

    private void ChangeColor(Color color)
    {
        // _meshRenderer.material.color = color;
        GetComponent<MeshRenderer>().material.color = color;
    }
}
