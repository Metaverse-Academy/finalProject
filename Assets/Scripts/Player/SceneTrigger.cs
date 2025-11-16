using UnityEngine;
using UnityEngine.UI;

public class MessageTrigger : MonoBehaviour
{
    [Header("Message Settings")]
    [TextArea(3, 5)]
    public string messageText = ""; // Message text

    [Header("Player UI References")]
    public PlayerUI[] playerUIs; // Array of UI references for each player

    private bool hasTriggered = false; // To prevent multiple triggers

    private void Start()
    {
        // Hide all panels at start
        foreach (var ui in playerUIs)
        {
            if (ui.messagePanel != null)
            {
                ui.messagePanel.SetActive(false);
                Debug.Log($"🟢 Message panel hidden for player UI: {ui.playerTag}");
            }
            else
            {
                Debug.LogError($"❌ Message panel missing for player UI: {ui.playerTag}");
            }

            if (ui.skipButton != null)
            {
                // Capture variable for correct reference in loop
                var capturedUI = ui;
                ui.skipButton.onClick.AddListener(() => SkipMessage(capturedUI));
                Debug.Log($"🔗 Skip button linked for player UI: {ui.playerTag}");
            }
            else
            {
                Debug.LogError($"❌ Skip button missing for player UI: {ui.playerTag}");
            }
        }

      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log($"🚶‍♂️ Player '{other.name}' entered trigger.");
            ShowMessageForPlayer(other.tag);
             Cursor.lockState = CursorLockMode.None;
             Cursor.visible = true;
            hasTriggered = true;
        }
    }

    void ShowMessageForPlayer(string playerTag)
    {
        foreach (var ui in playerUIs)
        {
            if (ui.playerTag == playerTag)
            {
                if (ui.messagePanel == null || ui.messageText == null)
                {
                    Debug.LogError($"❌ Missing UI references for player tag: {playerTag}");
                    return;
                }

                Debug.Log($"📢 Showing message to player '{playerTag}': \"{messageText}\"");

                ui.messageText.text = messageText;
                ui.messagePanel.SetActive(true);
                return;
            }
        }

        Debug.LogWarning($"⚠️ No UI found for player tag: {playerTag}");
    }

    void SkipMessage(PlayerUI ui)
    {
        if (ui.messagePanel != null && ui.messagePanel.activeSelf)
        {
            ui.messagePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log($"⏭️ Player '{ui.playerTag}' skipped the message.");
        }
        else
        {
            Debug.Log($"⚠️ No active message to skip for player '{ui.playerTag}'.");
        }
    }
}

[System.Serializable]
public class PlayerUI
{
    public string playerTag;         // Example: "Player1", "Player2"
    public GameObject messagePanel;  // The player's message panel
    public Text messageText;         // The text component
    public Button skipButton;        // The skip button
}
