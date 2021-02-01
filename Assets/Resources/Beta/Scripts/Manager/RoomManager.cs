using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using System.IO;


namespace BeanSupreme.v1
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        public static RoomManager I;

        public Dictionary<string, object> Settings = new Dictionary<string, object>();
        public GameObject[] playerSpawns;
        public GameObject[] objectSpawns;
        public int spawnItems = 3;
        public List<string> spawnableObjects;
        public ObjectManager om;

        private void setup()
        {

            if (I)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            I = this;
            //BasicGunSettings
            Settings.Add("PistolSpeed", 1f);
            Settings.Add("PistolFireRate", 1f);
            Settings.Add("PistolClipSize", 6);
            Settings.Add("PistolMaxRound", 120);
            Settings.Add("PistolSpawnRounds", 18);
            Settings.Add("PistolReloadTime", 1f);
            Settings.Add("SMGSpeed", 0.7f);
            Settings.Add("SMGFireRate", 0.3f);
            Settings.Add("SMGClipSize", 32);
            Settings.Add("SMGMaxRound", 320);
            Settings.Add("SMGSpawnRounds", 160);
            Settings.Add("SMGReloadTime", 0.7f);
            Settings.Add("ShotgunSpeed", 0.3f);
            Settings.Add("ShotgunFireRate", 1f);
            Settings.Add("ShotgunClipSize", 2);
            Settings.Add("ShotgunMaxRound", 16);
            Settings.Add("ShotgunSpawnRounds", 8);
            Settings.Add("ShotgunReloadTime", 1f);
            Settings.Add("ShotgunSpread", 15f);
            Settings.Add("ShotgunAmount", 10);
            Settings.Add("BulletSpeedFactor", 0.5f);
            Settings.Add("BulletSlowFactor", 0.999f);
            Settings.Add("BulletDropSpeed", 0.5f);

            //BasicPlayerSettings
            Settings.Add("PlayerMoveSpeed", 5.0f); //FIX THIS
                                                   //Running
            Settings.Add("PlayerRunFactor", 1.5f);
            Settings.Add("PlayerStaminaMax", 10f);
            Settings.Add("PlayerStaminaRecovery", 0.1f);
            //Sneaky
            Settings.Add("PlayerCrouchFactor", 0.75f);
            Settings.Add("PlayerCrawlFactor", 0.5f);
            //Existing
            Settings.Add("PlayerMaxHealth", 100f);
            Settings.Add("PlayerMaxInventory", 12);
            Settings.Add("PlayerSquishiness", 0.8f);
            //FlightControl
            Settings.Add("PlayerJumpStrength", 10f);
            Settings.Add("PlayerDriftControl", 0.02f);
            //Drink me eat me
            Settings.Add("PlayerScaleFactor", 0.7f);
            Settings.Add("PlayerCrouchScaleFactor", 0.5f);
            Settings.Add("PlayerCrawlScaleFactor", 0.3f);

            //Environmental
            Settings.Add("BaseSquishiness", 1f);
            Settings.Add("DamageFactor", 20f);
            Settings.Add("BaseLightBrightness", 0.5f);
            Settings.Add("FlashlightBrightness", 1f);
            Settings.Add("BaseFog", 0.5f);

            Settings.Add("KillsToWin", 10);

            //g = PhotonNetwork.Instantiate(, spawnpoint.position, spawnpoint.rotation);
            string basepath = "Beta\\Prefabs";
            spawnableObjects.Add(Path.Combine(basepath, PistolObject.Prefab));
            spawnableObjects.Add(Path.Combine(basepath, SMGObject.Prefab));
            spawnableObjects.Add(Path.Combine(basepath, ShotgunObject.Prefab));
        }

        private void Awake()
        {
        }
        // Start is called before the first frame update
        void Start()
        {
            if(om != null) om.spawnItems();
            foreach(GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (go.name == "Lights")
                {
                    for(int i = 0; i < go.transform.childCount; i++)
                    {
                        go.transform.GetChild(i).GetComponent<Light>().intensity = (float)Settings["BaseLightBrightness"];
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Winner")) if ((int)PhotonNetwork.CurrentRoom.CustomProperties["Winner"] > -1) doWin();
        }

        public override void OnEnable()
        {

            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        // Announce a player connected

        // When a player leaves, clean up

        // Set Spawn Points

        // Spawn objects

        public void doWin()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Destroy(PlayerManager.I.gameObject);
            Cursor.lockState=CursorLockMode.None;
            SceneManager.LoadScene(0);
            PhotonNetwork.Destroy(gameObject);
        }

        public void checkWin()
        {
            int hasWon = -1;
            foreach(Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (p.CustomProperties.ContainsKey("score")) if (!((int)p.CustomProperties["score"] < (int)Settings["KillsToWin"]))
                {
                    hasWon = p.ActorNumber;
                }
            }
            if(hasWon>-1)
            {
                ExitGames.Client.Photon.Hashtable newHT = new ExitGames.Client.Photon.Hashtable();
                newHT["Winner"] = hasWon;
                PhotonNetwork.CurrentRoom.SetCustomProperties(newHT);
            }
        }
        void OnSceneLoaded(Scene s, LoadSceneMode l)
        {
            if (s.buildIndex == 1)
            {
                setup();
                
                GameObject[] AllObj = SceneManager.GetActiveScene().GetRootGameObjects();
                GameObject[] Objects = new GameObject[0];
                GameObject[] Players = new GameObject[0];
                foreach (GameObject go in AllObj)
                {
                    switch (go.name)
                    {
                        case "PlayerSpawns":
                            {
                                Transform[] t = go.transform.GetComponentsInChildren<Transform>();
                                Players = new GameObject[t.Length];
                                int i = 0;
                                foreach (Transform go2 in t)
                                {
                                    Players[i] = go2.gameObject;
                                    i++;
                                }
                                break;
                            }
                        case "ObjectSpawns":
                            {
                                Transform[] t = go.transform.GetComponentsInChildren<Transform>();
                                Objects = new GameObject[t.Length];
                                int i = 0;
                                foreach (Transform go2 in t)
                                {
                                    Objects[i] = go2.gameObject;
                                    i++;
                                }
                                break;
                            }
                    }

                }
                playerSpawns = Players;
                objectSpawns = Objects;
                spawnItems = (int)((Objects.Length) / 2f);

                if (PhotonNetwork.LocalPlayer.IsMasterClient) om = (PhotonNetwork.Instantiate(Path.Combine("Beta\\Prefabs", "ObjectManager"), Vector3.zero, Quaternion.identity)).GetComponent<ObjectManager>();                
                
                PhotonNetwork.Instantiate(Path.Combine("Beta\\Prefabs", "PlayerManager.v1"), Vector3.zero, Quaternion.identity);                

                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {

                Cursor.lockState = CursorLockMode.None;
            }
        }

        // Spawn objectives

        // Detect a win, leave room and return to menu
    }
}