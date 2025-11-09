using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users; // <-- „Â„

public class LockDevicesOnJoin : MonoBehaviour
{
    void OnEnable()
    {
        var pim = PlayerInputManager.instance ?? FindObjectOfType<PlayerInputManager>();
        if (pim != null) pim.onPlayerJoined += OnPlayerJoined;
    }

    void OnDisable()
    {
        var pim = PlayerInputManager.instance ?? FindObjectOfType<PlayerInputManager>();
        if (pim != null) pim.onPlayerJoined -= OnPlayerJoined;
    }

    void OnPlayerJoined(PlayerInput input)
    {
        // «›’· √Ì √ÃÂ“… ”«»ﬁ… ⁄‰ Â–« «··«⁄»
        input.user.UnpairDevices();

        if (input.playerIndex == 0)
        {
            // ·«⁄» «·ﬂÌ»Ê—œ
            if (Keyboard.current != null)
                InputUser.PerformPairingWithDevice(Keyboard.current, input.user);
            if (Mouse.current != null)
                InputUser.PerformPairingWithDevice(Mouse.current, input.user);

            input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }
        else if (input.playerIndex == 1)
        {
            // «· ﬁÿ ‰›” «·Ìœ «· Ì ÷€ÿ  ··«‰÷„«„ (√÷„‰ „‰ Gamepad.current)
            var pad = input.devices.FirstOrDefault(d => d is Gamepad) as Gamepad;
            if (pad == null) pad = Gamepad.current;

            if (pad == null)
            {
                Debug.LogError("·«  ÊÃœ Ìœ „ ’·…! «÷€ÿÌ “— ›Ì «·Ìœ ··«‰÷„«„.");
                return;
            }

            InputUser.PerformPairingWithDevice(pad, input.user);
            input.SwitchCurrentControlScheme("Gamepad", pad);
        }
    }
}

