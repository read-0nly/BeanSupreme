using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Chat;
using System.IO;
using eg = ExitGames.Client.Photon;
namespace BeanSupreme.v1
{
    public class TeamObject : _Object
    {
        public Color color;
        public override void use()
        {
            Debug.Log("Painting");

            transform.parent.parent.parent.parent.GetComponent<PhotonView>().RPC("paint", RpcTarget.All, color.r, color.g, color.b);

            eg.Hashtable newHT = new eg.Hashtable();
            newHT["Team"] = new float[]{ color.r, color.g, color.b};
            PhotonNetwork.LocalPlayer.SetCustomProperties(newHT);
            base.use();
        }
    }
}