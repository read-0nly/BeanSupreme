using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
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
            Settings.Add("PlayerScaleFactor", 0.95f);
            Settings.Add("PlayerCrouchScaleFactor", 0.75f);
            Settings.Add("PlayerCrawlScaleFactor", 0.50f);

            //Environmental
            Settings.Add("BaseSquishiness", 1f);
            Settings.Add("DamageFactor", 20f);
            Settings.Add("BaseLightBrightness", 1f);
            Settings.Add("FlashlightBrightness", 1f);
            Settings.Add("BaseFog", 0.5f);

            //g = PhotonNetwork.Instantiate(, spawnpoint.position, spawnpoint.rotation);
            string basepath = "Beta\\Prefabs";
            spawnableObjects.Add(Path.Combine(basepath, PistolObject.Prefab));
            spawnableObjects.Add(Path.Combine(basepath, SMGObject.Prefab));
        }

        private void Awake()
        {
        }
        // Start is called before the first frame update
        void Start()
        {
            om.spawnItems();
        }

        // Update is called once per frame
        void Update()
        {

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

        void OnSceneLoaded(Scene s, LoadSceneMode l)
        {
            if (s.buildIndex == 1)
            {
                setup();
                GameObject[] AllObj = SceneManager.GetActiveScene().GetRootGameObjects();
                ArrayList plaspa = new ArrayList(); 
                ArrayList objspa = new ArrayList();
                foreach (GameObject go in AllObj)
                {
                    switch (go.name)
                    {
                        case "SpawnPlayer":
                            {
                                plaspa.Add(go);
                                break;
                            }
                        case "SpawnObject":
                            {
                                objspa.Add(go);
                                break;
                            }
                    }

                }
                playerSpawns = new GameObject[plaspa.Count];
                objectSpawns = new GameObject[objspa.Count];
                spawnItems = (int)(objspa.Count);
                for (int i = 0; i < playerSpawns.Length; i++) { playerSpawns[i] = (GameObject)(plaspa[i]); }
                for (int i = 0; i < objectSpawns.Length; i++) { objectSpawns[i] = (GameObject)(objspa[i]); }
                PhotonNetwork.Instantiate(Path.Combine("Beta\\Prefabs", "PlayerManager.v1"), Vector3.zero, Quaternion.identity);
                om = (PhotonNetwork.Instantiate(Path.Combine("Beta\\Prefabs", "ObjectManager"), Vector3.zero, Quaternion.identity)).GetComponent<ObjectManager>();

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