using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float mouseSensitivity = 2f;
    private float _cameraVerticalRotation = 0f;
    private float inputX;
    private float inputY;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0.7f, 0.3f);
    
    // Start is called before the first frame update
    void Start()
    {
        // Lock and hide the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Take input from the user:
        inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate the camera around its local x axis
        _cameraVerticalRotation -= inputY;
        _cameraVerticalRotation = Mathf.Clamp(_cameraVerticalRotation, -90f, 70f); // restrict the rotation between the given value
        transform.localEulerAngles = Vector3.right * _cameraVerticalRotation; // Vector3.right == new Vector3(1, 0, 0)
        
        // Rotate the player object and the camera around their y axis
        player.Rotate(Vector3.up * inputX); // Vector3.up == new Vector3(0, 1, 0)
    }
}
