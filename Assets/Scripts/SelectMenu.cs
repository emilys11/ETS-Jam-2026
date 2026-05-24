using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] GameObject thisPanel;

    [SerializeField] Button[] menuSelectItems = new Button[3];

    int selectedMenuItem = 0;

    InputActions controls;
    InputAction joystick;
    InputAction button1;

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
                MenuManager.Instance.StartGame(GameManager.DifficultyEnum.Easy);
                break;
            case 1:
                MenuManager.Instance.StartGame(GameManager.DifficultyEnum.Hard);
                break;
            case 2:
                MenuManager.Instance.StartGame(GameManager.DifficultyEnum.Apocalypse);
                break;
            default:
                break;
        }
        thisPanel.SetActive(false);
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
        menuSelectItems[0].Select();
    }

    void OnDisable()
    {
        joystick.Disable();
        button1.Disable();
    }
}
