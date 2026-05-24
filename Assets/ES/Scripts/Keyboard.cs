using UnityEngine;
using TMPro;

public class Keyboard : MonoBehaviour
{

    [SerializeField] private TMP_InputField inputField;

    public void OpenKeyboard()
    {
        Debug.Log("OPEN KEYBOARD");
        System.Diagnostics.Process.Start("OSK.exe");
        inputField.ActivateInputField();
    }
}
