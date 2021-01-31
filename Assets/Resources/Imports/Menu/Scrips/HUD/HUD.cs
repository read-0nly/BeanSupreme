using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace MultiMenu
{
    public class HUD : MonoBehaviour
    {
        public static HUD I;
        public Text Status;
        private void Awake()
        {
            if (I)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            I = this;
        }
            // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}