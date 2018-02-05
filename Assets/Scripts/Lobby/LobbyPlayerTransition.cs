using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Lobby {
    [RequireComponent(typeof(PlayerController))]
    public class LobbyPlayerTransition: NetworkBehaviour {

        public override void OnStartLocalPlayer() {
            //Time.timeScale = 0;

            Debug.Log(PlayerUI.localPlayer.curPlayer);

            if (PlayerUI.localPlayer.curPlayer > 0) {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = PlayerUI.localPlayer.curPlayer;
                pc.playerClass = PlayerUI.localPlayer.curClass;

                transform.position = new Vector3(-30, 50, -9 + 3 * PlayerUI.localPlayer.curPlayer);

                Destroy(this);
            } else {
                SceneManager.LoadScene("VRTestScene", LoadSceneMode.Additive);

                RpcStartPlaying();
            }
        }

        [ClientRpc]
        void RpcStartPlaying()
        {
            //Time.timeScale = 1;
            Destroy(gameObject);  
        }

    }
}