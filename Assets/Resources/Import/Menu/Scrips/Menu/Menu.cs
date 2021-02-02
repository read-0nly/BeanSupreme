using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace MultiMenu
{
    public class Menu : MonoBehaviourPunCallbacks
    {
        public static Menu instance;
        public GameObject[] menuPanels;
        public Dictionary<string, GameObject> menuDict = new Dictionary<string, GameObject>();
        public string preferredNick = "";
        // Start is called before the first frame update


        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            instance = this;
        }
        void Start()
        {
            foreach (GameObject g in menuPanels)
            {
                menuDict.Add(g.name, g);
                g.SetActive(false);
            }
            menuDict["LoadingMenu"].SetActive(true);

            PhotonNetwork.GameVersion = "MenuTestv1";
            PhotonNetwork.ConnectUsingSettings();
            setMenu("MainMenu");

        }

        public void quitGame()
        {
            Debug.Log("Quitting");
            Application.Quit(0);
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void setMenu(string s)
        {
            foreach (GameObject g in menuPanels)
            {
                g.SetActive(false);
            }
            menuDict[s].SetActive(true);
            menuDict[s].GetComponent<MenuPage>().refresh();
        }


        public override void OnConnected()
        {
            PhotonNetwork.LocalPlayer.NickName = (string.IsNullOrEmpty(preferredNick) ? "Player " + Random.Range(0, 1000).ToString("0000") : preferredNick);
            HUD.I.Status.text = "Current Region: " + PhotonNetwork.CloudRegion;
            base.OnConnected();
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
        }

        public override void OnJoinedRoom()
        {
            setMenu("RoomMenu");
            base.OnJoinedRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
        }
    }
}