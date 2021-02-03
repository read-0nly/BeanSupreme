using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

namespace BeanSupreme.v1
{

    public class SniperObject : ScopedFireableObject
    {
        public new static string Prefab = "SniperObject";
        // Start is called before the first frame update
        public override void Start()
        {
            setup();
            scopeFOV = 15;
            camSens = 2f;

        }

        public override void setup()
        {
            PistolSpeed = (float)RoomManager.I.Settings["SniperSpeed"];
            pistol = g;
            //BasicGunSettings
            FireRate = (float)RoomManager.I.Settings["SniperFireRate"];
            clipRounds = (int)RoomManager.I.Settings["SniperClipSize"];
            clipSize = (int)RoomManager.I.Settings["SniperClipSize"];
            totalRounds = (int)RoomManager.I.Settings["SniperSpawnRounds"];
            reloadDuration = (float)RoomManager.I.Settings["SniperReloadTime"];
            base.setup();
        }
    }
}