using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

namespace BeanSupreme.v1
{

    public class SMGObject : FireableObject
    {
        public new static string Prefab = "SMGObject";
        // Start is called before the first frame update
        public override void Start()
        {
            setup();
            base.Start();
        }

        public override void setup()
        {
            PistolSpeed = (float)RoomManager.I.Settings["SMGSpeed"];
            pistol = g;
            //BasicGunSettings
            FireRate = (float)RoomManager.I.Settings["SMGFireRate"];
            clipRounds = (int)RoomManager.I.Settings["SMGClipSize"];
            clipSize = (int)RoomManager.I.Settings["SMGClipSize"];
            totalRounds = (int)RoomManager.I.Settings["SMGSpawnRounds"];
            reloadDuration = (float)RoomManager.I.Settings["SMGReloadTime"];
            base.setup();
        }
    }
}