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
            if (!PV.IsMine) return;
            Debug.Log("Painting");

            if (transform.parent.parent.parent) try { transform.parent.parent.parent.gameObject.GetComponent<Renderer>().material.color=color; } catch { }


            eg.Hashtable newHT = new eg.Hashtable();
            newHT["Team"] = new float[]{ color.r, color.g, color.b};
            PhotonNetwork.LocalPlayer.SetCustomProperties(newHT);
            base.use();
        }
    }
}