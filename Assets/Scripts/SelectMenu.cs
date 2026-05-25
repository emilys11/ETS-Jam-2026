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
        ColorBlock cb;
        foreach (Button button in menuSelectItems)
        {
            cb = button.colors;
            cb.colorMultiplier = 1.0f;
            button.colors = cb;
        }
        cb = menuSelectItems[selectedMenuItem].colors;
        cb.colorMultiplier = 1.5f;
        menuSelectItems[selectedMenuItem].colors = cb;
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

    //FOR CLICKING
    public void SelectEasy()
    {
        MenuManager.Instance.StartGame(GameManager.DifficultyEnum.Easy);
        thisPanel.SetActive(false);
    }

    public void SelectHard()
    {
        MenuManager.Instance.StartGame(GameManager.DifficultyEnum.Hard);
        thisPanel.SetActive(false);
    }

    public void SelectApocalypse()
    {
        MenuManager.Instance.StartGame(GameManager.DifficultyEnum.Apocalypse);
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

        ColorBlock cb;
        cb = menuSelectItems[selectedMenuItem].colors;
        cb.colorMultiplier = 1.5f;
        menuSelectItems[selectedMenuItem].colors = cb;
    }

    void OnDisable()
    {
        joystick.Disable();
        button1.Disable();
    }
}
