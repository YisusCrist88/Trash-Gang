using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunPlayer : MonoBehaviour
{
    SpriteRenderer playerRend;
    Color playerBaseColor;
    
    private void Awake()
    {
        playerRend = GetComponent<SpriteRenderer>();
        playerBaseColor = playerRend.color;


    }

    public void StunEffect(float stunTime)
    {
        

        playerRend.color = Color.yellow;
        Invoke(nameof(ResetStun), stunTime);
    }

    private void ResetStun()
    {
        playerRend.color = playerBaseColor;
    }
}
