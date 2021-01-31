using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

namespace BeanSupreme.v1
{

    public class PistolObject : FireableObject
    {
        public new static string Prefab = "PistolObject";
        // Start is called before the first frame update
        public override void Start()
        {
            setup();
        }

        public override void setup()
        {
            base.setup();
            PistolSpeed = (float)RoomManager.I.Settings["PistolSpeed"];
            pistol = g;
            //BasicGunSettings
            FireRate = (float)RoomManager.I.Settings["PistolFireRate"];
            clipRounds = (int)RoomManager.I.Settings["PistolClipSize"];
            clipSize = (int)RoomManager.I.Settings["PistolClipSize"];
            totalRounds = (int)RoomManager.I.Settings["PistolSpawnRounds"];
            reloadDuration = (float)RoomManager.I.Settings["PistolReloadTime"];
        }
    }
}