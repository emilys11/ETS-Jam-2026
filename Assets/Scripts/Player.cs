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
    InputAction attackAction;

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

    private void Attack(InputAction.CallbackContext context)
    {
        spawner.SpawnMeteorite();
    }

    void Awake()
    {
        controls = new InputActions();
    }

    void OnEnable()
    {
        moveAction = controls.Player.Move;
        moveAction.Enable();

        attackAction = controls.Player.Attack;
        attackAction.Enable();
        attackAction.performed += Attack;
    }



    void OnDisable()
    {
        moveAction.Disable();

        attackAction.Disable();
        attackAction.performed -= Attack;
    }

    public Rigidbody Target { get => target;}
}
