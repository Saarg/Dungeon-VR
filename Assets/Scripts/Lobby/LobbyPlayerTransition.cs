using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Lobby {

    [RequireComponent(typeof(PlayerController))]
    public class LobbyPlayerTransition: NetworkBehaviour {

        [SerializeField, HideInInspector]
        private GameObject VR_Scripts;
        [SerializeField, HideInInspector]
        private GameObject VR_SDK;

        public override void OnStartLocalPlayer() {
            if (SceneManager.GetActiveScene().name != "NetworkTest") {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = Random.Range(0, 3);
                pc.CmdUpdatePlayerClass(pc.playerId);

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
                Destroy(GameObject.Find("GameUI"));

                SceneManager.LoadSceneAsync("VRNetworkTest", LoadSceneMode.Additive);

                Instantiate(VR_SDK);
                // Instantiate(VR_Scripts);

                NetworkServer.Destroy(gameObject);
            }
        }
    }
}