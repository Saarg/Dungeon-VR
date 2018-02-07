using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Lobby {
    [RequireComponent(typeof(PlayerController))]
    public class LobbyPlayerTransition: NetworkBehaviour {

        public override void OnStartLocalPlayer() {
            if (PlayerUI.localPlayer == null)
                Destroy(this);

            if (PlayerUI.localPlayer.curPlayer > 0) {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = PlayerUI.localPlayer.curPlayer;
                pc.playerClassID = (PlayerController.PlayerClassEnum) PlayerUI.localPlayer.curClass;

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