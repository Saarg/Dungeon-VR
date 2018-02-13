using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Lobby {
    [RequireComponent(typeof(PlayerController))]
    public class LobbyPlayerTransition: NetworkBehaviour {

        public override void OnStartLocalPlayer() {
            if (PlayerUI.localPlayer == null && !PlayerUI.gameMaster) {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = 1;
                pc.CmdUpdatePlayerClass(0);

                transform.position = new Vector3(0, 0, 0);

                Destroy(this);
                return;
            }

            if (PlayerUI.localPlayer.curPlayer > 0) {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = PlayerUI.localPlayer.curPlayer;
                pc.CmdUpdatePlayerClass(PlayerUI.localPlayer.curClass);

                transform.position = new Vector3(-23, 40, -9 + 3 * PlayerUI.localPlayer.curPlayer);

                Destroy(this);
            } else {
                SceneManager.LoadScene("VRTestScene", LoadSceneMode.Additive);

                Destroy(GameUI.instance.gameObject);         
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}