using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event EventHandler OnPlayerPosChange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float TestSpeed;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            GameData.Instance.playerPos.x += TestSpeed* Time.deltaTime;
            if (OnPlayerPosChange != null)
            {
                OnPlayerPosChange(this, EventArgs.Empty);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            GameData.Instance.playerPos.x -= TestSpeed * Time.deltaTime;
            if (OnPlayerPosChange != null)
            {
                OnPlayerPosChange(this, EventArgs.Empty);
            }
        }
    }
}
