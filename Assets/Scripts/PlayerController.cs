using UnityEngine;

// Attach to player/cart
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(h * speed * Time.deltaTime, 0, v * speed * Time.deltaTime);
    }
}

