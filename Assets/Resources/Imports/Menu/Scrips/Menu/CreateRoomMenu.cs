using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
namespace MultiMenu
{
    public class CreateRoomMenu : MenuPage
    {

        public GameObject RoomField;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void refresh()
        {

            if (!PhotonNetwork.InLobby) { PhotonNetwork.JoinLobby(); }
        }

        public override void submit()
        {
            PhotonNetwork.CreateRoom(RoomField.GetComponent<InputField>().text, new Photon.Realtime.RoomOptions());
        }
        public override void cancel()
        {
            if (PhotonNetwork.InLobby) { PhotonNetwork.LeaveLobby(); }
            base.cancel();
        }
    }
}