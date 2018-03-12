using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using VRTK.Examples.Archery;
using VRTK;

namespace Lobby {

    [RequireComponent(typeof(PlayerController))]
    public class LobbyPlayerTransition: NetworkBehaviour {

        string[] names = {"Ulrich", "Demagorgon", "Marvin", "Gertrude", "Astriel", "Simone"};

        [SerializeField]
        private GameObject VR_SDK;
        [SerializeField]
        private GameObject VR_Hand;

        public override void OnStartLocalPlayer() {
            if (SceneManager.GetActiveScene().name != "_Dungeon01") {
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

                gameObject.name = PlayerUI.localPlayer.pName.text;
                pc.CmdSetName(gameObject.name);           

                GameObject.Find("GameUI").GetComponent<GameUI>().isVr = false; 

                Destroy(this);
            } else {
                SceneManager.LoadSceneAsync("VRNetworkTest", LoadSceneMode.Additive);
                SceneManager.sceneLoaded += SetVRNetworkTestActive;                

                VRTK_SDKManager sdkManager = Instantiate(VR_SDK).GetComponent<VRTK_SDKManager>();
                Transform leftTarget = sdkManager.scriptAliasLeftController.transform;
                Transform rightTarget = sdkManager.scriptAliasRightController.transform;

                GameObject leftHand = Instantiate(VR_Hand);
                leftHand.name = "LeftHandNetworked";
                GameObject rightHand = Instantiate(VR_Hand);
                rightHand.name = "RightHandNetworked";                
                
                NetworkServer.SpawnWithClientAuthority(leftHand, gameObject);
                NetworkServer.SpawnWithClientAuthority(rightHand, gameObject);

                // Destroy the renderer for the vr player (he already has is hands ^^)
                Destroy(leftHand.GetComponentInChildren<Renderer>().gameObject);
                Destroy(rightHand.GetComponentInChildren<Renderer>().gameObject);

                leftHand.GetComponent<Follow>().target = leftTarget;
                rightHand.GetComponent<Follow>().target = rightTarget;

                sdkManager.enabled = true;

                GameObject.Find("GameUI").GetComponent<GameUI>().isVr = true;

                NetworkServer.Destroy(gameObject);
            }
        }

        void SetVRNetworkTestActive(Scene scene, LoadSceneMode lodMode) {
            if (scene.name.Equals("VRNetworkTest")) {
                SceneManager.SetActiveScene(scene);

                SceneManager.sceneLoaded -= SetVRNetworkTestActive;
            }
        }
    }
}
