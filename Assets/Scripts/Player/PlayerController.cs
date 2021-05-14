using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Flags]
    enum Inputs
    {
        NONE = 0,
        UP = 1 << 0,
        RIGHT = 1 << 1,
        DOWN = 1 << 2,
        LEFT = 1 << 3,
        RIGHT_RELEASE = 1 << 4,
        LEFT_RELEASE = 1 << 5,
        ALL = UP | RIGHT | DOWN | LEFT | RIGHT_RELEASE | LEFT_RELEASE
    }

    Inputs inputs = Inputs.NONE;

    public float TestSpeed, jumpPower, maxVelocity;

    public Animator animator;
    public Rigidbody2D rb;

    public event EventHandler OnPlayerStateChange;
    public event EventHandler OnUserInput;

    bool playerVelocityChanged = false, stopPlayer = false, jump = false;
    float playerVelocity = 0;

    private void Start()
    {
        OnUserInput+=HandleOnUserInput;
    }
    //Input System
    void Update()
    {
        Inputs pre_inputs = inputs;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputs |= Inputs.RIGHT;
        }
        else
        {
            inputs &= Inputs.RIGHT ^ Inputs.ALL;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputs |= Inputs.LEFT;
        }
        else
        {
            inputs &= Inputs.LEFT ^ Inputs.ALL;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputs |= Inputs.UP;
        }
        else
        {
            inputs &= Inputs.UP ^ Inputs.ALL;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            inputs |= Inputs.DOWN;
        }
        else
        {
            inputs &= Inputs.DOWN ^ Inputs.ALL;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            inputs = Inputs.RIGHT_RELEASE;
        }
        else
        {
            inputs &= Inputs.RIGHT_RELEASE ^ Inputs.ALL;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            inputs = Inputs.LEFT_RELEASE;
        }
        else
        {
            inputs &= Inputs.LEFT_RELEASE ^ Inputs.ALL; 
        }

        if (inputs != pre_inputs)
        {
            OnUserInput(this, EventArgs.Empty);
        }

       
    }

    //Movement
    void FixedUpdate()
    {
        if (jump)
        {
            rb.AddForce(Vector2.up * jumpPower);
            GameData.Instance.playerState = GameData.PlayerState.JUMP;
            if (OnPlayerStateChange != null)
            {
                OnPlayerStateChange(this, EventArgs.Empty);
            }
            jump = false;
            landed = false;
        }
        if (stopPlayer)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            GameData.Instance.playerState = GameData.PlayerState.IDLE;
            stopPlayer = false;
            playerVelocity = 0.0f;
        }
        rb.AddForce(Vector2.right * playerVelocity);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
       

        if (landed)
        {
            if (rb.velocity.x < 0.1f && rb.velocity.x > -0.1f)
            {
                GameData.Instance.playerState = GameData.PlayerState.IDLE;
                if (OnPlayerStateChange != null)
                {
                    OnPlayerStateChange(this, EventArgs.Empty);
                }
            }
            else if (rb.velocity.x > 0.1f)
            {
                GameData.Instance.playerState = GameData.PlayerState.RUN_RIGHT;
                if (OnPlayerStateChange != null)
                {
                    OnPlayerStateChange(this, EventArgs.Empty);
                }
            }
            else
            {
                GameData.Instance.playerState = GameData.PlayerState.RUN_LEFT;
                if (OnPlayerStateChange != null)
                {
                    OnPlayerStateChange(this, EventArgs.Empty);
                }
            }
        }




        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    rb.AddForce(Vector2.right * TestSpeed);
        //    if (OnPlayerStateChange != null)
        //    {
        //        OnPlayerStateChange(this, EventArgs.Empty);
        //    }
        //}
        //else if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    rb.AddForce(Vector2.left * TestSpeed);
        //if (OnPlayerStateChange != null)
        //{
        //    OnPlayerStateChange(this, EventArgs.Empty);
        //}
        //}
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    rb.AddForce(Vector2.up * jumpPower);
        //}
    }

    bool landed = true;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            landed = true;
        }
    }
    //Input Check and State Machine
    void HandleOnUserInput(object sender, EventArgs e)
    {
        if (inputs.HasFlag(Inputs.RIGHT) && inputs.HasFlag(Inputs.LEFT))
        {
            inputs &= (Inputs.LEFT | Inputs.RIGHT) ^ Inputs.ALL;
        }


        if (inputs != Inputs.NONE)
        {
            if (inputs.HasFlag(Inputs.RIGHT))
            {
                playerVelocity = TestSpeed;
            }
            else if (inputs.HasFlag(Inputs.LEFT))
            {
                playerVelocity = -TestSpeed;
            }

            if (inputs.HasFlag(Inputs.UP))
            {
                jump = true;
            }
            if (inputs.HasFlag(Inputs.DOWN))
            {
                //
            }
            if (inputs.HasFlag(Inputs.RIGHT_RELEASE) || inputs.HasFlag(Inputs.LEFT_RELEASE))
            {
                if (GameData.Instance.playerState != GameData.PlayerState.JUMP)
                {
                    stopPlayer = true;
                }
                else
                {
                    playerVelocity = 0;
                }
            }
            

        }

        

    }
}
