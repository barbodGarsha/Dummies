using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event EventHandler OnPlayerStateChange;
    public class OnPlayerStateChangeArgs : EventArgs 
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float TestSpeed, jumpPower, maxVelocity;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector2.right * TestSpeed);
            this.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(this.GetComponent<Rigidbody2D>().velocity, maxVelocity);

            GameData.Instance.playerPos.x += TestSpeed* Time.deltaTime;
            if (OnPlayerStateChange != null)
            {
                OnPlayerStateChange(this, EventArgs.Empty);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector2.left * TestSpeed);
            this.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(this.GetComponent<Rigidbody2D>().velocity, maxVelocity);
            GameData.Instance.playerPos.x -= TestSpeed * Time.deltaTime;
            if (OnPlayerStateChange != null)
            {
                OnPlayerStateChange(this, EventArgs.Empty);
            }
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
             this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower);
        }
    }
}
