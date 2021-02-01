using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace MultiMenu
{
    public class SettingsMenu : MenuPage
    {

        public InputField nickname;
        public ChatHUD chatHud;
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
            nickname.text = PhotonNetwork.LocalPlayer.NickName;
        }

        public override void submit()
        {
            Menu.instance.preferredNick = nickname.text;
            if (PhotonNetwork.IsConnected) { PhotonNetwork.LocalPlayer.NickName = nickname.text; }
            chatHud.reconnect(chatHud.Region, nickname.text);
            Menu.instance.setMenu("MainMenu");

        }
        public override void cancel()
        {
            if (PhotonNetwork.InLobby) { PhotonNetwork.LeaveLobby(); }
            base.cancel();
        }
    }
}