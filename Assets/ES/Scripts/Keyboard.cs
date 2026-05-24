using UnityEngine;
using TMPro;

public class Keyboard : MonoBehaviour
{

    [SerializeField] private TMP_InputField inputField;

    public void OpenKeyboard()
    {
        System.Diagnostics.Process.Start("OSK.exe");
        inputField.ActivateInputField();
    }
}
