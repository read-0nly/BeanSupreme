using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace MultiMenu
{
    public class RoomListItem : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ConnectSelf);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ConnectSelf()
        {
            string server = GetComponentInChildren<Text>().text;
            PhotonNetwork.JoinRoom(server);
        }
    }
}