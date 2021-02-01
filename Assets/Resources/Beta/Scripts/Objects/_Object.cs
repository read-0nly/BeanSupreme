using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

namespace BeanSupreme.v1
{

    /*
     * An object is a thing that exists - it projects itself into the world using it's prefab
     */
    public abstract class _Object : MonoBehaviourPunCallbacks, IOnPhotonViewControllerChange
    {


        //This is the projection of any object in the world, including PlayerObject
        public float health;
        public PhotonView PV;
        public float MaxHealth = 100f;
        public float Squishiness;
        public float BaseLightBrightness;
        public GameObject g;
        public static string Prefab = "GenericObject";
        public Transform parentCam;
        // Start is called before the first frame update
        public virtual void Start()
        {
            try
            {
                PV = this.gameObject.GetComponent<PhotonView>();
            }
            catch { };
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (PV.IsMine)
            {
                look();
            }

        }

        //When something takes damage, damage is based off a speed*damageModifier
        //Faster bullets hurt more, as bullets go through things they lose velocity, when a certain velocity is reached the bullet dies
        [PunRPC]
        public void takeDamage(float damage)
        {
            if (PV.IsMine)
            {
                health -= damage;
                if (health > MaxHealth) health = MaxHealth;
                if (health < 0)
                {
                    PhotonView.Destroy(gameObject);
                }
            }
        }
        public virtual void look()
        {
            if (!parentCam) return;
            transform.rotation = (parentCam.name == "Head" ? Quaternion.Euler(transform.parent.rotation.eulerAngles) : parentCam.rotation);
        }
        public virtual void use() { }
        public virtual void toss() { }
        public virtual void grab(_Object o)
        {
        }
        public virtual void drop()
        {
        }

        [PunRPC]
        public virtual void snap(int pid, bool vis = false)
        {
            if (pid > -1)
            {
                if (PV.ObservedComponents.Contains(GetComponent<PhotonTransformView>()))
                    PV.ObservedComponents.Remove(GetComponent<PhotonTransformView>());
                makePhysical(false,vis);
                transform.parent = PhotonView.Find(pid).gameObject.transform.Find("Hand");
                if(transform.childCount>0) transform.GetChild(0).localPosition = new Vector3(0, 0, 0);
                transform.localPosition = new Vector3(0, 0, 0);
                transform.position = PhotonView.Find(pid).gameObject.transform.Find("Hand").position;
                transform.rotation = PhotonView.Find(pid).gameObject.transform.Find("Hand").rotation;
                parentCam = PhotonView.Find(pid).gameObject.transform;
                makePhysical(false,vis);
            }
            else
            {
                transform.parent = null;
                parentCam = null;
                if(!PV.ObservedComponents.Contains(GetComponent<PhotonTransformView>()))
                    PV.ObservedComponents.Add(GetComponent<PhotonTransformView>());
                makePhysical(true,vis);
            }
        }

        public void makePhysical( bool phys, bool vis)
        {
            try
            {
                if (gameObject.GetComponent<Rigidbody>()) {gameObject.GetComponent<Rigidbody>().isKinematic = !phys; }
                else { gameObject.GetComponentInChildren<Rigidbody>().isKinematic = !phys; }
            }
            catch { }
            try
            {
                if (gameObject.GetComponent<BoxCollider>()) { gameObject.GetComponent<BoxCollider>().isTrigger = !phys; }
                else { gameObject.GetComponentInChildren<BoxCollider>().isTrigger = !phys; }
            }
            catch { }

            gameObject.SetActive(vis);
        }

        public float sendDamage(float speed,int actor)
        {
            float newSpeed = (speed * Squishiness);
            float damage = newSpeed * (float)RoomManager.I.Settings["BaseDamageFactor"];
            return newSpeed;
        }

        public virtual void setup()
        {
            PV = gameObject.GetComponent<PhotonView>();
            g = this.gameObject;
            health = MaxHealth;
        }

        //spawn at selected spawnpoint
        public virtual bool spawn(Transform spawnpoint)
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual bool die()
        {
            try
            {
                if (PV.IsMine)
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public void OnControllerChange(Player newController, Player previousController)
        {
            
        }


    }
}