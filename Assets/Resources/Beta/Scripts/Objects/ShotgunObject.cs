using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

namespace BeanSupreme.v1
{

    public class ShotgunObject : FireableObject
    {
        public new static string Prefab = "ShotgunObject";

        private float shotgunSpread = 0.5f;
        private int shotgunAmount = 10;
        // Start is called before the first frame update
        public override void Start()
        {
            setup();
        }

        public override void setup()
        {
            PistolSpeed = (float)RoomManager.I.Settings["ShotgunSpeed"];
            pistol = g;
            //BasicGunSettings
            FireRate = (float)RoomManager.I.Settings["ShotgunFireRate"];
            clipRounds = (int)RoomManager.I.Settings["ShotgunClipSize"];
            clipSize = (int)RoomManager.I.Settings["ShotgunClipSize"];
            totalRounds = (int)RoomManager.I.Settings["ShotgunSpawnRounds"];
            reloadDuration = (float)RoomManager.I.Settings["ShotgunReloadTime"];
            shotgunSpread = (float)RoomManager.I.Settings["ShotgunSpread"];
            shotgunAmount = (int)RoomManager.I.Settings["ShotgunAmount"];
            base.setup();
        }
        public override void use()
        {
            if (Time.time - LastFire > FireRate && !hasFired)
            {
                if (clipRounds > 0 && !isReloading)
                {
                    for (int i = 0; i < shotgunAmount; i++)
                    {
                        shotgunFire();
                    }
                    LastFire = Time.time;
                    clipRounds--;
                }
                else if (!isReloading)
                {
                    startReload(false);
                }
            }
        }

        public void shotgunFire()
        {
            Quaternion q = bulletAnchor.rotation;
            q.eulerAngles = new Vector3(
                q.eulerAngles.x + Random.Range(-shotgunSpread, shotgunSpread),
                q.eulerAngles.y + Random.Range(-shotgunSpread, shotgunSpread),
                q.eulerAngles.z + Random.Range(-shotgunSpread, shotgunSpread)
                );
            GameObject Bullet = PhotonNetwork.Instantiate(Path.Combine("Beta\\Prefabs", "Bullet"),
                bulletAnchor.position +
                bulletAnchor.forward.normalized * 0.5f, q);
            Bullet.GetComponent<BulletBehavior>().bulletSpeed = BulletSpeedFactor;
            Bullet.GetComponent<BulletBehavior>().bulletDrop = (float)RoomManager.I.Settings["BulletSlowFactor"];
        }
    }
}