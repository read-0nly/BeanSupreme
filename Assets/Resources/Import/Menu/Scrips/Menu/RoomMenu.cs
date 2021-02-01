using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

namespace MultiMenu
{
    public class RoomMenu : MenuPage
    {
        // Start is called before the first frame update
        public GameObject TitleField;
        public GameObject PlayerList;
        public GameObject PlayerListItem;
        public GameObject startButton;

        void Start()
        {
            if(!Photon.Pun.PhotonNetwork.LocalPlayer.IsMasterClient) startButton.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void refresh()
        {
            TitleField.GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;
            updateRoomList();
            base.refresh();
        }

        public override void submit()
        {
            Debug.Log("Game Start");
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            base.submit();
        }

        public override void cancel()
        {
            PhotonNetwork.LeaveRoom();
            base.cancel();
        }
        private void updateRoomList()
        {
            for (int i = 0; i < PlayerList.transform.childCount; i++)
            {
                Destroy(PlayerList.transform.GetChild(i).gameObject);
            }
            foreach (Player s in PhotonNetwork.PlayerList)
            {
                GameObject _new = Instantiate(PlayerListItem, PlayerList.transform);
                Text t = _new.GetComponent<Text>();
                t.text = (s.IsLocal ? "- " + s.NickName + " -" : s.NickName);
                t.color = (s.IsMasterClient ? Color.cyan : (s.IsInactive ? Color.gray : Color.white));

            }

        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {

            base.OnRoomPropertiesUpdate(propertiesThatChanged);
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            updateRoomList();
            base.OnPlayerEnteredRoom(newPlayer);
        }
        public override void OnPlayerLeftRoom(Player oldPlayer)
        {
            updateRoomList();
            base.OnPlayerLeftRoom(oldPlayer);
        }
    }
}