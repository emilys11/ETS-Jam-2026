using UnityEngine;
using UnityEngine.InputSystem;

public class CreditMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;

    InputActions controls;
    InputAction button1;
    InputAction button2;

    private void ReturnToMainMenu(InputAction.CallbackContext context)
    {
        mainPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void PetDinosaur(InputAction.CallbackContext context)
    {
        
    }

    void Awake()
    {
        controls = new InputActions();
    }

    void OnEnable()
    {
        button1 = controls.Player.Button1;
        button1.Enable();
        button1.performed += ReturnToMainMenu;

        button2 = controls.Player.Button2;
        button2.Enable();
        button2.performed += PetDinosaur;
    }

    void OnDisable()
    {
        button1.Disable();
        button2.Disable();
    }
}
