using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    VisualElement menuContent;
    VisualElement idPopup;
    VisualElement instructionsPopup;

    TextField nicknameField;
    Label warningLabel;

    ServerNoteAPI serverAPI;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        menuContent = root.Q<VisualElement>("content-container");
        idPopup = root.Q<VisualElement>("id-popup");
        instructionsPopup = root.Q<VisualElement>("instructions-popup");

        nicknameField = root.Q<TextField>("player-id-field");
        warningLabel = root.Q<Label>("id-warning");

        var startButton = root.Q<Button>("btn-start");
        var instructionsButton = root.Q<Button>("btn-instructions");
        var exitButton = root.Q<Button>("btn-exit");

        var confirmButton = root.Q<Button>("btn-confirm-id");
        var cancelButton = root.Q<Button>("btn-cancel-id");
        var closeInstructionsButton = root.Q<Button>("btn-close-instructions");

        serverAPI = GetComponent<ServerNoteAPI>();
        if (serverAPI == null)
        {
            Debug.LogError("ServerNoteAPI not found on UI GameObject");
            return;
        }

        warningLabel.style.display = DisplayStyle.None;
        idPopup.style.display = DisplayStyle.None;
        instructionsPopup.style.display = DisplayStyle.None;

        startButton.clicked += OpenIdPopup;
        cancelButton.clicked += CloseIdPopup;
        confirmButton.clicked += OnConfirmClicked;

        instructionsButton.clicked += OpenInstructions;
        closeInstructionsButton.clicked += CloseInstructions;

        exitButton.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };

        nicknameField.RegisterValueChangedCallback(FilterNicknameInput);
    }

    void FilterNicknameInput(ChangeEvent<string> evt)
    {
        string filtered = "";

        foreach (char c in evt.newValue)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                filtered += c;
        }

        if (filtered != evt.newValue)
            nicknameField.SetValueWithoutNotify(filtered);
    }

    void OpenIdPopup()
    {
        idPopup.style.display = DisplayStyle.Flex;
        menuContent.SetEnabled(false);
        menuContent.style.opacity = 0.3f;
        nicknameField.Focus();
    }

    void CloseIdPopup()
    {
        idPopup.style.display = DisplayStyle.None;
        menuContent.SetEnabled(true);
        menuContent.style.opacity = 1f;
        warningLabel.style.display = DisplayStyle.None;
    }

    void OpenInstructions()
    {
        instructionsPopup.style.display = DisplayStyle.Flex;
        menuContent.SetEnabled(false);
        menuContent.style.opacity = 0.3f;
    }

    void CloseInstructions()
    {
        instructionsPopup.style.display = DisplayStyle.None;
        menuContent.SetEnabled(true);
        menuContent.style.opacity = 1f;
    }

    void OnConfirmClicked()
    {
        string nickname = nicknameField.value.Trim().ToLower();

        if (!ValidateNickname(nickname, out string error))
        {
            ShowWarning(error);
            return;
        }

        StartCoroutine(
            serverAPI.CheckSenderExists(nickname, exists =>
            {
                if (exists)
                {
                    ShowWarning("Please choose another nickname");
                }
                else
                {
                    PlayerPrefs.SetString("PlayerNickname", nickname);
                    SceneManager.LoadScene("Rooms");
                }
            })
        );
    }

    bool ValidateNickname(string nickname, out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(nickname))
        {
            error = "Please enter a nickname";
            return false;
        }

        if (nickname.Length < 3 || nickname.Length > 12)
        {
            error = "Nickname must be 3–12 characters";
            return false;
        }

        foreach (char c in nickname)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
            {
                error = "Only letters, numbers and _ allowed";
                return false;
            }
        }

        return true;
    }

    void ShowWarning(string message)
    {
        warningLabel.text = message;
        warningLabel.style.display = DisplayStyle.Flex;
    }
}
