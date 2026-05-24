using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class MainMenu : MonoBehaviour
{
    InputActions controls;
    InputAction joystick;
    InputAction button1;

    [SerializeField] Button[] menuSelectItems = new Button[3];
    [SerializeField] GameObject thisPanel;
    [SerializeField] GameObject creditPanel;
    [SerializeField] GameObject difficultyPanel;
    [SerializeField] Animator cassetteAnimator;
    int selectedMenuItem = 0;

    private void NavigateMenu(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().y < 0)
        {
            selectedMenuItem++;
        }
        else if (context.ReadValue<Vector2>().y > 0)
        {
            selectedMenuItem--;
        }

        if (selectedMenuItem >= menuSelectItems.Length)
        {
            selectedMenuItem = 0;
        }
        else if (selectedMenuItem < 0)
        {
            selectedMenuItem = menuSelectItems.Length - 1;
        }
        menuSelectItems[selectedMenuItem].Select();
    }

    private void SelectMenuItem(InputAction.CallbackContext context)
    {
        switch (selectedMenuItem)
        {
            case 0:
                cassetteAnimator.SetTrigger("ToLvlSelect");
                break;
            case 1:
                creditPanel.SetActive(true);
                thisPanel.SetActive(false);
                break;
            case 2:
                Application.Quit();
                break;
            default:
                break;
        }
    }

    void Awake()
    {
        controls = new InputActions();
    }

    void OnEnable()
    {
        joystick = controls.Player.Move;
        joystick.Enable();
        joystick.performed += NavigateMenu;

        button1 = controls.Player.Button1;
        button1.Enable();
        button1.performed += SelectMenuItem;

        selectedMenuItem = 0;
        menuSelectItems[0].Select();
    }

    void OnDisable()
    {
        joystick.Disable();
        button1.Disable();
    }
}
