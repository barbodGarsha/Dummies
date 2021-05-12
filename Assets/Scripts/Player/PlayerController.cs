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

    bool playerVelocityChanged = false;

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
        if (playerVelocityChanged)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
            playerVelocityChanged = false;
        }
        switch (GameData.Instance.playerState)
        {
            case GameData.PlayerState.IDLE:
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                break;
            case GameData.PlayerState.RUN_RIGHT:
                rb.AddForce(Vector2.right * TestSpeed);
                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
                break;
            case GameData.PlayerState.RUN_LEFT:
                rb.AddForce(Vector2.left * TestSpeed);
                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
                break;
            case GameData.PlayerState.JUMP:
                rb.AddForce(Vector2.up * jumpPower);
                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
                break;
            case GameData.PlayerState.PUSH_RIGHT:
                break;
            default:
                break;
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
         //    if (OnPlayerStateChange != null)
         //    {
         //        OnPlayerStateChange(this, EventArgs.Empty);
         //    }
         //}
         //if (Input.GetKeyDown(KeyCode.UpArrow))
         //{
         //    rb.AddForce(Vector2.up * jumpPower);
         //}
    }

    //Input Check and State Machine
    void HandleOnUserInput(object sender, EventArgs e)
    {
        if (inputs.HasFlag(Inputs.RIGHT) && inputs.HasFlag(Inputs.LEFT))
        {
            inputs &= (Inputs.LEFT | Inputs.RIGHT) ^ Inputs.ALL;
        }

        if (inputs.HasFlag(Inputs.RIGHT_RELEASE))
        {
            if (GameData.Instance.playerState != GameData.PlayerState.RUN_RIGHT)
            {
                inputs &= Inputs.RIGHT_RELEASE ^ Inputs.ALL;
            }
        }

        if (inputs.HasFlag(Inputs.LEFT_RELEASE))
        {
            if (GameData.Instance.playerState != GameData.PlayerState.RUN_LEFT)
            {
                inputs &= Inputs.LEFT_RELEASE ^ Inputs.ALL;
            }
        }

        if (inputs.HasFlag(Inputs.UP))
        {
            //For Jumping once
        }

        if (inputs != Inputs.NONE)
        {
            if (inputs.HasFlag(Inputs.RIGHT))
            {
                rb.AddForce(Vector2.right * TestSpeed);
                GameData.Instance.playerState = GameData.PlayerState.RUN_RIGHT;
            }
            if (inputs.HasFlag(Inputs.LEFT))
            {
                rb.AddForce(Vector2.left * TestSpeed);
                GameData.Instance.playerState = GameData.PlayerState.RUN_LEFT;
            }
            if (inputs.HasFlag(Inputs.UP))
            {
                rb.AddForce(Vector2.up * jumpPower);

                GameData.Instance.playerState = GameData.PlayerState.JUMP;
            }
            if (inputs.HasFlag(Inputs.DOWN))
            {
                //
            }
            if (inputs.HasFlag(Inputs.RIGHT_RELEASE) || inputs.HasFlag(Inputs.LEFT_RELEASE))
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                GameData.Instance.playerState = GameData.PlayerState.IDLE;
            }
            
        }

        

    }
}
