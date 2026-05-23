using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 10.0f;
    [SerializeField] Collider worldBorderCollider;
    [SerializeField] PlayerSpawner spawner;
    [SerializeField] GameObject target;

    Vector4 worldBounds;

    Rigidbody rb;
    
    InputActions controls;
    InputAction moveAction;
    InputAction attackAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        spawner.Player = this;

        Bounds bounds = worldBorderCollider.bounds;
        worldBounds = new Vector4(
            bounds.center.x - bounds.extents.x,
            bounds.center.x + bounds.extents.x,
            bounds.center.y - bounds.extents.y,
            bounds.center.y + bounds.extents.y
        );
    }

    void Update()
    {
        MoveCamera();
        CheckBounds();
    }

    void MoveCamera()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        rb.linearVelocity = new Vector3(movement.x, 0.0f, movement.y) * cameraSpeed;
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

    public GameObject Target { get => target;}
}
