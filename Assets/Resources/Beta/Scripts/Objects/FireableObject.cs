using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
namespace BeanSupreme.v1
{

    public abstract class FireableObject : _Object
    {
        public float DamageFactor;
        public float FlashlightBrightness;
        public float BulletSpeedFactor;
        public float BulletSlowFactor;
        public float BulletDropSpeed;
        public float PistolSpeed;
        public GameObject pistol;
        public float LastFire;
        public float FireRate = 0.1f;
        public bool hasFired = false;
        public float reloadTime = 0;
        public float reloadDuration = 1;
        public bool isReloading = false;
        public int clipRounds = 6;
        public int clipSize = 6;
        public int totalRounds = 24;
        // Start is called before the first frame update
        public override void Start()
        {
        }

        public override void setup()
        {
            base.setup();
            BulletSpeedFactor = (float)RoomManager.I.Settings["BulletSpeedFactor"];// 0.5f);
            BulletSlowFactor = (float)RoomManager.I.Settings["BulletSlowFactor"];// 0.999f);
            BulletDropSpeed = (float)RoomManager.I.Settings["BulletDropSpeed"];// 0.5f);
            DamageFactor = (float)RoomManager.I.Settings["DamageFactor"];// 10f);
            FlashlightBrightness = (float)RoomManager.I.Settings["FlashlightBrightness"];// 1f);
        }
        // Update is called once per frame
        public override void Update()
        {
            if (PV.IsMine)
            {
                look();
            }

        }

        public override void use()
        {
            if (Time.time - LastFire > FireRate && !hasFired)
            {
                if (clipRounds > 0 && !isReloading)
                {
                    GameObject Bullet = PhotonNetwork.Instantiate(Path.Combine("Beta\\Prefabs", "Bullet"),
                        transform.GetChild(0).position +
                        transform.forward.normalized * 0.5f, transform.rotation);
                    Bullet.GetComponent<BulletBehavior>().bulletSpeed = BulletSpeedFactor;
                    Bullet.GetComponent<BulletBehavior>().bulletDrop = (float)RoomManager.I.Settings["BulletSlowFactor"];
                    LastFire = Time.time;
                    clipRounds--;
                }
                else if (!isReloading)
                {
                    startReload();
                }
            }
        }
        public virtual void startReload()
        {
            reloadTime = Time.time;
            isReloading = true;
        }
        public virtual void reloadCheck()
        {

            if (isReloading && totalRounds > 0)
            {
                int delta = (totalRounds>clipSize?clipSize:totalRounds);
                delta = (clipRounds + delta > clipSize ? clipSize - clipRounds : delta);
                clipRounds += delta;
                totalRounds -= delta;
                isReloading = false;
            }
        }
        public override void look()
        {
            if (Time.time - reloadTime > reloadDuration)
            {
                reloadCheck();
                base.look();
            }
            else
            {
                Vector3 par = transform.parent.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(par.x + 30, par.y, par.z);
            }
        }
        public virtual void fire() { 

        }
    }
}