using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeanSupreme.v1
{
    public class BulletColliderBehavior : MonoBehaviour
    {
        public float speed=0.5f;
        public float baseDamage = 10;

        // Start is called before the first frame update
        void Start()
        {
            baseDamage = (float) RoomManager.I.Settings["DamageFactor"];
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (this.transform.parent.GetComponent<BulletBehavior>().GetComponent<Photon.Pun.PhotonView>().IsMine) { 
                RaycastHit rh = new RaycastHit();
                Physics.Raycast(new Ray(this.transform.position, this.transform.up * speed), out rh, speed*speed);
                if (rh.collider)
                {
                    OnTriggerEnter(rh.collider);
                }
            }

        }


        private void OnTriggerEnter(Collider other)
        {
            if (!(other.CompareTag("Bullet") || other.CompareTag("Object")))
            {
                try
                {
                    PlayerObject pc = other.transform.parent.gameObject.GetComponent<PlayerObject>();
                    if (pc)
                    {
                        pc.PV.RPC("TakeDamage", Photon.Pun.RpcTarget.All, 10, pc.PV.Owner.ActorNumber, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);

                    }
                    else
                    {

                        //other.gameObject.GetComponent<Renderer>().material.SetColor("_Color",Color.green);
                    }
                }
                catch
                {

                }
                if (this.transform.parent.GetComponent<BulletBehavior>().GetComponent<Photon.Pun.PhotonView>().IsMine)
                    Photon.Pun.PhotonNetwork.Destroy(this.transform.parent.gameObject);
            }

        }
    }
}