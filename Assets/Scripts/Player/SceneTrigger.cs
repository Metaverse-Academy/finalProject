using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageTrigger : MonoBehaviour
{
    [Header("Message Settings")]
    public float messageDurationLeft = 15f; // مدة عرض الرسالة بالثواني
    public float messageDurationRight = 15f; // مدة عرض الرسالة بالثواني
    private float timerLeft = 0f;
    private float timerRight = 0f;
    
    [Header("UI References")]
    public GameObject messagePanelRight; // Panel for Player2 (معدّل)
    public GameObject messagePanelLeft; // Panel for Player1 (معدّل)


    private void Start()
    {
        messagePanelRight.SetActive(false);
        messagePanelLeft.SetActive(false);

        timerLeft = messageDurationLeft;
        timerRight = messageDurationRight;

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
        //if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            // Try to get PlayerID component
            PlayerID playerID = other.GetComponent<PlayerID>();
            
            if (playerID != null)
            {
                if (playerID.playerNumber == 1)
                {
                    messagePanelLeft.SetActive(true);
                }
                else if (playerID.playerNumber == 2)
                {
                    messagePanelRight.SetActive(true);
                }
            }
        }
    }

    
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
}