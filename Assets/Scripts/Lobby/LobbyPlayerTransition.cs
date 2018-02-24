using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Lobby {

    [RequireComponent(typeof(PlayerController))]
    public class LobbyPlayerTransition: NetworkBehaviour {

        string[] names = {"Ulrich", "Demagorgon", "Marvin", "Gertrude", "Astriel", "Simone"};

        [SerializeField, HideInInspector]
        private GameObject VR_Scripts;
        [SerializeField, HideInInspector]
        private GameObject VR_SDK;

        public override void OnStartLocalPlayer() {
            if (SceneManager.GetActiveScene().name != "NetworkTest") {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = Random.Range(0, 4);
                pc.CmdUpdatePlayerClass(pc.playerId);

                transform.position = new Vector3(0, 0, 0);

                gameObject.name = names[Random.Range(0, names.Length)];
                pc.CmdSetName(gameObject.name);

                Destroy(this);
                return;
            }

            if (PlayerUI.localPlayer.curPlayer > 0) {
                PlayerController pc = GetComponent<PlayerController>();

                pc.playerId = PlayerUI.localPlayer.curPlayer;
                pc.CmdUpdatePlayerClass(PlayerUI.localPlayer.curClass);

                GameObject StartPos = GameObject.FindGameObjectsWithTag("StartPos")[pc.playerId-1];
                transform.position = StartPos.transform.position;

                gameObject.name = names[Random.Range(0, names.Length)];
                pc.CmdSetName(gameObject.name);           

                GameObject.Find("GameUI").GetComponent<GameUI>().isVr = false;   

                Destroy(this);
            } else {
                SceneManager.LoadSceneAsync("VRNetworkTest", LoadSceneMode.Additive);

                Instantiate(VR_SDK);

                GameObject.Find("GameUI").GetComponent<GameUI>().isVr = true;

                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
