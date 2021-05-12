using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : ScriptableObject
{
    static GameData _instance;
	public static GameData Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = FindObjectOfType<GameData>();
			}
			if (!_instance)
			{
				_instance = CreateInstance<GameData>();
			}
			return _instance;
		}
	}

	public Vector2 playerPos;

    public enum PlayerState
    {
		IDLE,
		RUN_RIGHT,
		RUN_LEFT,
		JUMP,
		PUSH_RIGHT
	}

	public PlayerState playerState = PlayerState.IDLE;
}
