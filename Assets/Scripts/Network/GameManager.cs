using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public static GameManager instance;

	[SyncVar(hook="SetStartTime")] float startTime;
	[SerializeField]
	[SyncVar] float buildTime = 300;
	[SerializeField]
	[SyncVar] float gameTime = 600;

	[SyncVar] bool _gamePhase;
	public bool gamePhase { get { return _gamePhase; } }
	public delegate void OnStartGame();
    public OnStartGame onStartGame;
	public UnityEvent StartGame;

	[SyncVar] bool _buildPhase;
	public bool buildPhase { get { return _buildPhase; } }
	public delegate void OnStartBuild();
    public OnStartBuild onStartBuild;
	public UnityEvent StartBuild;

	GameUI gameUI;

	void Awake()
	{
		instance = this;
		gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
	}

	public override void OnStartServer() {
		startTime = Time.realtimeSinceStartup;

		_buildPhase = true;

		if (onStartBuild != null)
			onStartBuild.Invoke();
		StartBuild.Invoke();
	}

	void Update()
	{
		if (buildPhase && Time.realtimeSinceStartup - startTime > buildTime) {
			startTime = Time.realtimeSinceStartup;

			if (onStartGame != null)
				onStartGame.Invoke();
			StartGame.Invoke();

			_buildPhase = false;
			_gamePhase = true;
		}

		if (gamePhase && Time.realtimeSinceStartup - startTime > gameTime) {
			_buildPhase = false;
			_gamePhase = false;
		}

		if (gamePhase) {
			gameUI.UpdateGamemodeUI("Game mode", gameTime - (Time.realtimeSinceStartup - startTime));
		} else if (buildPhase) {
			gameUI.UpdateGamemodeUI("Build mode", buildTime - (Time.realtimeSinceStartup - startTime));
		}
	}

	void SetStartTime(float t) {
		startTime = Time.realtimeSinceStartup;
	}
}
