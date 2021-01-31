using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace BeanSupreme.v1 { 

public class ObjectManager : MonoBehaviour
    {
        public static ObjectManager I;

        PhotonView pv;
        List<GameObject> objectsInWorld = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

            if (I)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            I = this;
        }


        public void spawnItems()
        {
            pv = GetComponent<PhotonView>();
            if (!pv.IsMine) return;

            for (int i = 0; i < RoomManager.I.spawnItems; i++)
            {
                int itemI = Random.Range(0, RoomManager.I.spawnableObjects.Count);
                int itemL = Random.Range(0, RoomManager.I.objectSpawns.Length);
                objectsInWorld.Add(PhotonNetwork.Instantiate(
                    RoomManager.I.spawnableObjects[itemI],
                    RoomManager.I.objectSpawns[itemL].transform.position,
                    RoomManager.I.objectSpawns[itemL].transform.rotation
                    ));
            }

        }

        // Update is called once per frame
        void Update()
    {

    }
}
}