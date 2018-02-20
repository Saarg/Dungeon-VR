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
                pc.CmdUpdatePlayerClass(3);

                transform.position = new Vector3(0, 0, 0);

                Destroy(this);
                return;
            }

            if (PlayerUI.localPlayer.curPlayer > 0) {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = PlayerUI.localPlayer.curPlayer;
                pc.CmdUpdatePlayerClass(PlayerUI.localPlayer.curClass);

                transform.position = new Vector3(-30, 50, -9 + 3 * PlayerUI.localPlayer.curPlayer);

                Destroy(this);
            } else {
                SceneManager.LoadScene("VRTestScene", LoadSceneMode.Additive);

                NetworkServer.Destroy(gameObject);
                Destroy(GameObject.Find("GameUI"));
            }
        }
    }
}