using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 10.0f;
    [SerializeField] PlayerSpawner spawner;
    [SerializeField] Rigidbody target;

    Vector4 worldBounds;
    
    InputActions controls;
    InputAction moveAction;
    InputAction meteoriteAction;
    InputAction volcanoAction;

    void Start()
    {
        spawner.Player = this;

        worldBounds = new Vector4(-1000.0f, 1000.0f, -1000.0f, 1000.0f);
    }

    void Update()
    {
        MoveTarget();
        CheckBounds();
    }

    void MoveTarget()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        target.linearVelocity = movement * cameraSpeed;
    }

    void CheckBounds()
    {
        float xPos = Mathf.Clamp(transform.position.x, worldBounds.x, worldBounds.y);
        float zPos = Mathf.Clamp(transform.position.z, worldBounds.z, worldBounds.w);
        transform.position = new Vector3(xPos, transform.position.y, zPos);
    }

    private void Meteorite(InputAction.CallbackContext context)
    {
        spawner.SpawnMeteorite();
    }

    private void Volcano(InputAction.CallbackContext context)
    {
        spawner.SpawnVolcano();
    }

    void Awake()
    {
        controls = new InputActions();
    }

    void OnEnable()
    {
        moveAction = controls.Player.Move;
        moveAction.Enable();

        meteoriteAction = controls.Player.Button1;
        meteoriteAction.Enable();
        meteoriteAction.performed += Meteorite;

        volcanoAction = controls.Player.Button2;
        volcanoAction.Enable();
        volcanoAction.performed += Volcano;
    }

    void OnDisable()
    {
        moveAction.Disable();

        meteoriteAction.Disable();
        meteoriteAction.performed -= Meteorite;

        volcanoAction.Disable();
        volcanoAction.performed -= Volcano;
    }

    public Rigidbody Target { get => target;}
}
