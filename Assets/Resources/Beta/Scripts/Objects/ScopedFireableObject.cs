using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Chat;
using System.IO;


namespace BeanSupreme.v1
{
    public abstract class ScopedFireableObject : FireableObject
    {
        public float scopeFOV = 30;
        public float camSens = 2f;
        public bool scoped = false;
        public override void use2()
        {
            Camera c = parentCam.GetComponentInChildren<Camera>();
            if (scoped)
            {
                c.fieldOfView = 60;
                PlayerManager.I.GetComponent<PlayerControl>().camSens = PlayerControl.camsensMax;
            }
            else
            {
                c.fieldOfView = scopeFOV;
                PlayerManager.I.GetComponent<PlayerControl>().camSens = camSens;

            }
            scoped = !scoped;
            

        }
        public override void drop()
        {
            PlayerManager.I.Player.GetComponent<PlayerObject>().cam.fieldOfView = 60;
            PlayerManager.I.GetComponent<PlayerControl>().camSens = PlayerControl.camsensMax;
            base.drop();
        }
    }
}
