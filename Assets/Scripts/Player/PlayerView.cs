using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameData.Instance.playerPos = this.gameObject.transform.position;
        PlayerController player_controller = GetComponent<PlayerController>();
        player_controller.OnPlayerPosChange += HandleOnPlayerPosChange;
    }

    void HandleOnPlayerPosChange(object sender, EventArgs e) 
    {
        SyncPosition();
    }

    void SyncPosition()
    {
        this.gameObject.transform.position = GameData.Instance.playerPos;
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
