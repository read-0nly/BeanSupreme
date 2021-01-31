using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

namespace MultiMenu
{
    public class FindRoomMenu : MenuPage
    {
        // Start is called before the first frame update

        public GameObject RoomListObj;
        public GameObject Button;
        public List<Photon.Realtime.RoomInfo> rooms;
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

            base.refresh();
        }

        public override void submit()
        {

            base.submit();
        }

        public override void cancel()
        {
            if (PhotonNetwork.InLobby) { PhotonNetwork.LeaveLobby(); }
            base.cancel();
        }

        public void joinRoom(GameObject g)
        {
            PhotonNetwork.JoinRoom(g.GetComponent<Text>().text);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            rooms = roomList;
            Debug.Log(rooms[0].Name);
            foreach (RoomInfo roomentry in rooms)
            {
                GameObject _listItem = Instantiate(Button, RoomListObj.transform);
                _listItem.GetComponentInChildren<Text>().text = roomentry.Name;
            }
            base.OnRoomListUpdate(roomList);
        }
    }
}