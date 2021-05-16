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

    public Rigidbody2D rb;
    public BoxCollider2D boxColider;

    public event EventHandler OnPlayerStateChange;
    public event EventHandler OnUserInput;

    bool stopPlayer = false, jump = false, push = false, landed = true;
    float playerVelocity = 0;

    void Start()
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

    //Movement and State Machine
    void FixedUpdate()
    {
        if (jump)
        {
            if (landed)
            {
                rb.AddForce(Vector2.up * jumpPower);
                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
                GameData.Instance.playerState = GameData.PlayerState.JUMP;
                if (OnPlayerStateChange != null)
                {
                    OnPlayerStateChange(this, EventArgs.Empty);
                }
                landed = false;
            }
            jump = false;

        }
        if (stopPlayer)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            GameData.Instance.playerState = GameData.PlayerState.IDLE;
            stopPlayer = false;
            playerVelocity = 0.0f;
        }
        if (playerVelocity != 0)
        {
            rb.AddForce(Vector2.right * playerVelocity);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
        }
        if (landed)
        {
            if (rb.velocity.x < 0.1f && rb.velocity.x > -0.1f && GameData.Instance.playerState!= GameData.PlayerState.IDLE)
            {
                GameData.Instance.playerState = GameData.PlayerState.IDLE;
                if (OnPlayerStateChange != null)
                {
                    OnPlayerStateChange(this, EventArgs.Empty);
                }
            }
            else if (rb.velocity.x > 0.1f && GameData.Instance.playerState != GameData.PlayerState.RUN_RIGHT)
            {
                if (!push)
                {
                    GameData.Instance.playerState = GameData.PlayerState.RUN_RIGHT;
                    if (OnPlayerStateChange != null)
                    {
                        OnPlayerStateChange(this, EventArgs.Empty);
                    }
                }
            }
            else if(rb.velocity.x < -0.1f && GameData.Instance.playerState != GameData.PlayerState.RUN_LEFT)
            {
                GameData.Instance.playerState = GameData.PlayerState.RUN_LEFT;
                if (OnPlayerStateChange != null)
                {
                    OnPlayerStateChange(this, EventArgs.Empty);
                }
            }
            if (push && GameData.Instance.playerState == GameData.PlayerState.RUN_RIGHT)
            {
                GameData.Instance.playerState = GameData.PlayerState.PUSH_RIGHT;
                if (OnPlayerStateChange != null)
                {
                    OnPlayerStateChange(this, EventArgs.Empty);
                }
            }
        }
        
    }

    void landedCheck()
    {
        float offsetX = 0.3f, offsetY = 0.1f;
        Vector2 bottomLeft = new Vector2(boxColider.bounds.center.x - boxColider.bounds.extents.x + offsetX, boxColider.bounds.center.y - boxColider.bounds.extents.y - offsetY);
        Vector2 bottomRight = new Vector2(boxColider.bounds.center.x + boxColider.bounds.extents.x - offsetX, boxColider.bounds.center.y - boxColider.bounds.extents.y - offsetY);

        if (Physics2D.OverlapArea(bottomLeft, bottomRight))
        {
            if (Physics2D.OverlapArea(bottomLeft, bottomRight).gameObject.name != "Player")
            {
                landed = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Box")
        {
            push = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 center = collider.bounds.center;

        bool right = contactPoint.x < center.x;

        if (collision.gameObject.name=="Ground")
        {
            landed = true;
        }
        if (collision.gameObject.name == "Box")
        {

            landedCheck();
            if (landed && right)
            {
                push = true;
            }
        }
    }
    //Input Check 
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
