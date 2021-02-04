using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BeanSupreme.v1
{
    public class GenericObject : _Object
    {
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        public override void use()
        {
            PV.RPC("snap", Photon.Pun.RpcTarget.All, PlayerManager.I.Player.GetComponent<PlayerObject>().head.GetComponent<Photon.Pun.PhotonView>().ViewID, false);
            this.makePhysical(false,true, true);
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.localPosition = new Vector3(0, 0, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log("Owner=" + PV.Owner.ActorNumber + ";Self=" + Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
        }
        public override void use2()
        {
            base.use();
            this.makePhysical(false, false, true);
            this.transform.localPosition = new Vector3(0, 0, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        public override void drop()
        {
            base.drop();
        }
    }
}