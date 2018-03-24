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
	[SyncVar] float timeLeft;
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

	PlayerController[] players;

	void Awake()
	{
		instance = this;
		gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();

		timeLeft = buildTime;
	}

	public override void OnStartServer() {
		startTime = Time.realtimeSinceStartup;

		_buildPhase = true;

		RpcStartBuild();
	}

	void Update()
	{
		if (_gamePhase) {
			if (isServer) {	
				timeLeft = gameTime - (Time.realtimeSinceStartup - startTime);
			}

			bool allDead = true;
			foreach (PlayerController pc in players)
			{
				if (!pc.dead) {
					allDead = false;
					break;
				}
			}

			if (allDead) {
				if (gameUI.isVr)
					gameUI.Win();
				else
					gameUI.Lose();

				gameObject.SetActive(false);
			} else if (timeLeft <= 0) {
				if (gameUI.isVr)
					gameUI.Lose();
				else
					gameUI.Win();

				gameObject.SetActive(false);
			}

			gameUI.UpdateGamemodeUI("Game mode", timeLeft);
		} else if (buildPhase) {
			if (isServer) {
				timeLeft = buildTime - (Time.realtimeSinceStartup - startTime);

				if (timeLeft < 0) {
					_buildPhase = false;
					_gamePhase = true;

					startTime = Time.realtimeSinceStartup;
					RpcStartGame();
				}
			}
			gameUI.UpdateGamemodeUI("Build mode", timeLeft);
		}
	}

	void SetStartTime(float t) {
		startTime = Time.realtimeSinceStartup;
	}

	[ClientRpc]
	void RpcStartBuild() {
		if (onStartBuild != null)
			onStartBuild.Invoke();
		StartBuild.Invoke();
	}

	[ClientRpc]
	void RpcStartGame() {
		players = FindObjectsOfType<PlayerController>();

		if (onStartGame != null)
			onStartGame.Invoke();
		StartGame.Invoke();
	}
}
