using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]

public class PlayerSoundEffects : MonoBehaviour
{
    private AudioSource audiosrc;

    void Start()
    {
        audiosrc = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();

        if (Input.GetKeyDown(KeyCode.Space)) audiosrc.Play(); //play sound, always when space is pressed, fix later;
        if (Input.GetMouseButtonDown(0)) audiosrc.Play(); //hit sound
        if(pm.GetIsWalking()) audiosrc.Play();
        if(pm.GetIsSprinting()) audiosrc.Play();
    }
}
