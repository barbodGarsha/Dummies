using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        GameData.Instance.playerPos = this.gameObject.transform.position;
        PlayerController player_controller = GetComponent<PlayerController>();
        player_controller.OnPlayerStateChange += HandleOnPlayerStateChange;
    }

    void HandleOnPlayerStateChange(object sender, EventArgs e) 
    {
        //AnimationControl
        switch (GameData.Instance.playerState)
        {
            case GameData.PlayerState.IDLE:
                animator.Play("Idle");
                break;
            case GameData.PlayerState.RUN_RIGHT:
                animator.Play("Run_Right");
                break;
            case GameData.PlayerState.RUN_LEFT:
                animator.Play("Run_Left");
                break;
            case GameData.PlayerState.JUMP:
                animator.Play("Jump");
                break;
            case GameData.PlayerState.PUSH_RIGHT:
                animator.Play("Push_Right");
                break;
            default:
                break;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // to keep the model updated while the unity physics system change the position
        if (this.transform.hasChanged)
        {
            GameData.Instance.playerPos = this.gameObject.transform.position;
        }
    }
}
