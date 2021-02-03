using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Chat;
using System.IO;


namespace BeanSupreme.v1
{
    public class PlayerControl : MonoBehaviour
    {
        PhotonView PV;
        PlayerObject Player;
        float deathTime;
        float deathCooldown = 10;
        public float camSens = 4; //How sensitive it with mouse
        public static float camsensMax = 4;
        bool equipping = false;
        bool dropping = false;
        bool switching = false;
        bool observing=false;
        bool fire2 = false;
        private void Awake()
        {
            PV = GetComponent<PhotonView>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if (PV.IsMine)
            {
                try
                {
                    Player = this.GetComponent<PlayerManager>().Player.GetComponent<PlayerObject>();
                }
                catch
                {
                    spawnPlayer();
                    this.GetComponent<PlayerManager>().Player = Player.gameObject;
                }
            }


        }
        void FixedUpdate()
        {
            if (!PV.IsMine)
            {
                return;
            }

            if (!Player)
            {
                if(Time.time - deathTime > deathCooldown) spawnPlayer();
            }
            else
            {
                if (!MultiMenu.ChatHUD.I.showInput)
                {
                    Player.move(System.Math.Sign(Input.GetAxisRaw("Horizontal")), System.Math.Sign(Input.GetAxisRaw("Vertical")), (Input.GetButton("Jump")), (Input.GetButton("Sprint")));

                    bool fire = Input.GetButton("Fire1");
                    if (fire)
                    {
                        Player.use();
                    }
                    bool fire2t = Input.GetButton("Fire2");
                    if (!fire2 && fire2t)
                    {
                        Player.use2();
                    }
                    fire2 = fire2t;
                    if (Input.GetButton("Equip"))
                    {
                        if (!equipping)
                        {
                            Ray ray = new Ray(Player.cam.transform.position, Player.cam.transform.forward);
                            RaycastHit hit = new RaycastHit();
                            Physics.Raycast(ray, out hit, 2f);
                            if (hit.collider)
                            {
                                Debug.Log(hit.collider.name);
                                Player.grab(hit.collider.gameObject.GetComponent<_Object>());
                            }
                            equipping = true;
                        }

                    }
                    else equipping = false;
                    if (Input.GetButton("Drop"))
                    {
                        if (!dropping)
                        {
                            Player.drop();
                            dropping = true;
                        }
                    }
                    else dropping = false;
                    if (Input.GetButton("Reload"))
                    {
                        Player.reload();
                    }
                    if (Input.GetButton("Stand"))
                    {
                        Player.stand();
                    }
                    if (Input.GetButton("Crouch"))
                    {
                        Player.crouch();
                    }
                    if (Input.GetButton("Crawl"))
                    {
                        Player.crawl();
                    }
                    if (Input.GetButton("Observe"))
                    {
                         if(observing == false)
                        {
                            Player.Observe(true);                            
                        }
                        observing = true;
                    }
                    else if (observing == true)
                    {
                        Player.Observe(false);
                        observing = false;
                    }
                    float wheel = Input.GetAxis("Mouse ScrollWheel");
                    if (wheel != 0)
                    {
                        if (!switching)
                        {
                            Player.swap(-System.Math.Sign(wheel));
                            switching = true;
                        }
                    }
                    else  switching = false;
                }
            }
        }
        private void Update()
        {

            if (!PV.IsMine)
            {
                return;
            }

            if (Player)
            {
                if (!MultiMenu.ChatHUD.I.showInput)
                {
                    Player.look(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), camSens);
                }
            }
        }
        void spawnPlayer()
        {
            foreach (GameObject gRoot in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (gRoot.name == "Camera") gRoot.GetComponent<Camera>().enabled = false;
            }
            GameObject[] gto = RoomManager.I.playerSpawns;
            int index = Random.Range(0, gto.Length);
            PlayerManager.I.spawnPlayer(gto[index].transform);
            Player = PlayerManager.I.Player.GetComponent<PlayerObject>();
            Player.spawn(gto[index].transform);
            
        } 

        public void Die()
        {
            Debug.Log("Dying");
            if (PV.IsMine)
            {
                foreach (GameObject gRoot in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (gRoot.name == "Camera") gRoot.GetComponent<Camera>().enabled = true;
                }
                
                deathTime = Time.time;
                Destroy(Player.GetComponent<PlayerObject>());
                PhotonNetwork.Destroy(Player.gameObject);
                Player = null;
                PlayerManager.I.Player = null;
            }

        }

    }
}