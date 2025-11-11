using UnityEngine;
using UnityEngine.InputSystem;
 
[RequireComponent(typeof(PlayerInputManager))]
public class Lobby : MonoBehaviour
{
    // [Header("Dependencies")]
    // PlayerInputManager inputManager;
 
    // [Header("Settings")]
    // public Material[] materials;
 
    // void Awake()
    // {
    //     inputManager = GetComponent<PlayerInputManager>();
    // }
 
    // public void OnPlayerJoined(PlayerInput input)
    // {
    //     var id = inputManager.playerCount - 1;
    //     var player = input.gameObject;
    //     player.transform.position = new(id, 1, 0);
    //     player.GetComponent<PlayerMovement>().SetUp(id, materials[id]);
    // }
}