using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]

public class PlayerSoundEffects : MonoBehaviour
{
    [SerializeField] private AudioSource audiosrc;
    [SerializeField] private PlayerMovement pm;


    private void Awake()
    {
        audiosrc = GetComponent<AudioSource>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space)) audiosrc.Play(); //play sound, always when space is pressed, fix later;
        // if (Input.GetMouseButtonDown(0)) audiosrc.Play(); //hit sound
        // if(pm.GetIsWalking()) audiosrc.Play();
        // if(pm.GetIsSprinting()) audiosrc.Play()
    }

    public void PlaySound()
    { 
        audiosrc.Play();
    }
}
