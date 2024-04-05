using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private Camera _cam; // we need this to know where the player is aiming at.
    private PlayerToGameControl _gameControl;
    // private Player playerClass; // This's the player's component of the player game object  
    [SerializeField] private LayerMask _playerLayerMask;
    
    [SerializeField] private float _hitDistance = 2.5f;

    private void Awake()
    {
        _playerLayerMask = LayerMask.GetMask("Player");
        _gameControl = FindObjectOfType<GameControl>().GetComponent<GameControl>();
        // playerClass = GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<PlayerLook>().GetCam(); // because we already have a camera on PLayerLook script which is attached to the player gameobject
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit()
    {
        Debug.Log("Hit attempted!");
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * _hitDistance, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hitTarget, _hitDistance, _playerLayerMask))
        {
            Debug.Log("A player was Hit!");
            _gameControl.HasHit(gameObject, hitTarget.collider.gameObject);
            // Debug.Log(hitTarget.collider.gameObject.name);
            // hitTarget.collider.GetComponent<MeshRenderer>().material.color = Color.red;
            // gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    public void GetHit(Vector3 hitDirection)
    {
        // playerClass.ChangeColor(Color.red); Prefer to do this in the Player class cuz u might get hit from non tagger
        transform.position += hitDirection + Vector3.up / 4;
    }
}
