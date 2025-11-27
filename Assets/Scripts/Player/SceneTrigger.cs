using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageTrigger : MonoBehaviour
{
   [Header("Message Settings")]
    public float messageDurationLeft = 15f;
    public float messageDurationRight = 15f;
    private float timerLeft = 0f;
    private float timerRight = 0f;
    
    [Header("UI References")]
    public GameObject messagePanelRight;
    public GameObject messagePanelLeft;
    
    [Header("Skip Button Settings")]
    public Button skipButtonLeft;
    public Button skipButtonRight;
    
    private void Start()
    {
        messagePanelRight.SetActive(false);
        messagePanelLeft.SetActive(false);
        timerLeft = messageDurationLeft;
        timerRight = messageDurationRight;
        
        // تأكد من أن الزر يعمل
        if (skipButtonLeft != null)
        {
            skipButtonLeft.onClick.RemoveAllListeners();
            skipButtonLeft.onClick.AddListener(() => {
                Debug.Log("Left button clicked!");
                OnSkipButtonLeftClicked();
            });
        }
        
        if (skipButtonRight != null)
        {
            skipButtonRight.onClick.RemoveAllListeners();
            skipButtonRight.onClick.AddListener(() => {
                Debug.Log("Right button clicked!");
                OnSkipButtonRightClicked();
            });
        }
    }
    
    private void Update()
    {
        if (messagePanelLeft.activeSelf)
        {
            timerLeft -= Time.deltaTime;
            if (timerLeft <= 0f)
            {
                messagePanelLeft.SetActive(false);
                timerLeft = messageDurationLeft;
            }
        }
        
        if (messagePanelRight.activeSelf)
        {
            timerRight -= Time.deltaTime;
            if (timerRight <= 0f)
            {
                messagePanelRight.SetActive(false);
                timerRight = messageDurationRight;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerID playerID = other.GetComponent<PlayerID>();
            if (playerID != null)
            {
                if (playerID.playerNumber == 1)
                {
                    messagePanelLeft.SetActive(true);
                    timerLeft = messageDurationLeft;
                    Debug.Log("Panel Left opened");
                }
                else if (playerID.playerNumber == 2)
                {
                    messagePanelRight.SetActive(true);
                    timerRight = messageDurationRight;
                    Debug.Log("Panel Right opened");
                }
            }
        }
    }
    
    public void OnSkipButtonLeftClicked()
    {
        Debug.Log("Skip Left Panel");
        messagePanelLeft.SetActive(false);
        timerLeft = messageDurationLeft;
    }
    
    public void OnSkipButtonRightClicked()
    {
        Debug.Log("Skip Right Panel");
        messagePanelRight.SetActive(false);
        timerRight = messageDurationRight;
    }
}
//     [Header("Message Settings")]
//     public float messageDurationLeft = 15f; // مدة عرض الرسالة بالثواني
//     public float messageDurationRight = 15f; // مدة عرض الرسالة بالثواني
//     private float timerLeft = 0f;
//     private float timerRight = 0f;
    
//     [Header("UI References")]
//     public GameObject messagePanelRight; // Panel for Player2 (معدّل)
//     public GameObject messagePanelLeft; // Panel for Player1 (معدّل)


//     private void Start()
//     {
//         messagePanelRight.SetActive(false);
//         messagePanelLeft.SetActive(false);

//         timerLeft = messageDurationLeft;
//         timerRight = messageDurationRight;

//     }

//     private void Update()
//     {
//         if (messagePanelLeft.activeSelf)
//         {
//             timerLeft -= Time.deltaTime;
//             if (timerLeft <= 0f)
//             {
//                 messagePanelLeft.SetActive(false);
//                 timerLeft = messageDurationLeft;
//             }
//         }

//         if (messagePanelRight.activeSelf)
//         {
//             timerRight -= Time.deltaTime;
//             if (timerRight <= 0f)
//             {
//                 messagePanelRight.SetActive(false);
//                 timerRight = messageDurationRight;
//             }
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         //if (hasTriggered) return;

//         if (other.CompareTag("Player"))
//         {
//             // Try to get PlayerID component
//             PlayerID playerID = other.GetComponent<PlayerID>();
            
//             if (playerID != null)
//             {
//                 if (playerID.playerNumber == 1)
//                 {
//                     messagePanelLeft.SetActive(true);
//                 }
//                 else if (playerID.playerNumber == 2)
//                 {
//                     messagePanelRight.SetActive(true);
//                 }
//             }
//         }
//     }

    
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         // Try to get PlayerID component
    //         PlayerID playerID = other.GetComponent<PlayerID>();
            
    //         if (playerID != null)
    //         {
    //             if (playerID.playerNumber == 1)
    //             {
    //                 messagePanelLeft.SetActive(false);
    //             }
    //             else if (playerID.playerNumber == 2)
    //             {
    //                 messagePanelRight.SetActive(false);
    //             }
                
    //         }
    //     }
    // }
