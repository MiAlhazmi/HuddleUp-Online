using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 250f;
    private float _horizontalInput;
    private float _verticalInput;
    private float _jumpInput;
    private Rigidbody _rb;
    private Vector3 _movementInput;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) _rb.AddForce(0, jumpForce, 0);
        _movementInput = new Vector3(_horizontalInput * speed, _jumpInput * jumpForce, _verticalInput * speed);

        // transform.position += _movementInput * Time.deltaTime;
        transform.Translate(_movementInput * Time.deltaTime);
    }
}