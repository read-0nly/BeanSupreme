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

    public class PlayerManager : MonoBehaviour
    {
        public PhotonView PV;
        public GameObject Player;
        public static PlayerManager I;
        public string playerPrefab = "PlayerAnchor";
        private void Awake()
        {
            if (I)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            I = this;
            PV = GetComponent<PhotonView>();

        }
        // Start is called before the first frame update
        void Start()
        {
            eg.Hashtable newHT = new eg.Hashtable();
            newHT = PhotonNetwork.LocalPlayer.CustomProperties;
            newHT["stance"] = 0;
            newHT["score"] = 0;
            newHT["falling"] = false;
            newHT["hasjump"] = false;
            newHT["health"] = 0;
            PhotonNetwork.LocalPlayer.SetCustomProperties(newHT);

        }

        public void spawnPlayer(Transform spawn)
        {

            Player = PhotonNetwork.Instantiate(Path.Combine("Beta\\Prefabs", playerPrefab), spawn.position, spawn.rotation);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}