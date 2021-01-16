using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;
    public Vector2 movement = Vector2.zero;

    void FixedUpdate()
    {
        movement = new Vector2(SimpleInput.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, SimpleInput.GetAxis("Vertical") * speed * Time.fixedDeltaTime);
        ClientSend.PlayerMovement(movement);

    }
}
