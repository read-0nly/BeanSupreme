
/* Player Object exists as an abstraction that persists between lives - it puppets it's instance in the world.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using eg = ExitGames.Client.Photon;
using System.IO;
using Photon.Realtime;

namespace BeanSupreme.v1
{

    public class PlayerObject : _Object, IPunOwnershipCallbacks
    {
        //Basic vars
        public List<_Object> inventory = new List<_Object>();
        public List<_Object> basicLoadout = new List<_Object>();


        //Instance Details
        public Camera cam;
        public Transform head;
        public Rigidbody body;
        public GameObject hand;
        public Photon.Pun.PhotonView pv;

        //Setting-based vars
        public float FlashlightBrightness;
        public int MaxInventory;
        public int CurrentItemIndex = -1;
        public float MoveSpeed;
        public float RunFactor;
        public float StaminaMax;
        public float StaminaRecovery;

        public float CrouchFactor;
        public float CrawlFactor;

        public float JumpStrength;
        public float DriftControl;

        public float ScaleFactor;
        public float CrouchScaleFactor;
        public float CrawlScaleFactor;
        public float scaleshift = 0.01f;
        private float stepThresh = 0.01f;
        private float dropThresh = -0.2f;
        private float stepFreq = 0.6f;
        private float stepFreqDefault = 0.6f;
        public float lastStep = 0f;
        public Vector2 lastMove = new Vector2(0f,0f);
        private float lastRise =0f;
        private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
        public UnityEngine.UI.Text Stats;
        FireableObject item;
        Vector3 defaulthead = new Vector3(0,0,0);
        public Renderer hat;
        public Renderer hatrim;
        private string statFString = @"Player: {0}
Health: {1}
Current Clip: {2}
Clip Size: {3}
Total Ammo: {4}
Score: {5}
Lives: {6}
";
        public List<AudioSource> sounds = new List<AudioSource>();
        private void Awake()
        {

            Prefab = "PlayerAnchor";
            //Loud settings from round
            MaxHealth = (float)RoomManager.I.Settings["PlayerMaxHealth"];
            health = MaxHealth;
            MaxInventory = (int)RoomManager.I.Settings["PlayerMaxInventory"];
            Squishiness = (float)RoomManager.I.Settings["PlayerSquishiness"];



            MoveSpeed = (float)RoomManager.I.Settings["PlayerMoveSpeed"]; ;
            RunFactor = (float)RoomManager.I.Settings["PlayerRunFactor"]; ;
            StaminaMax = (float)RoomManager.I.Settings["PlayerStaminaMax"]; ;
            StaminaRecovery = (float)RoomManager.I.Settings["PlayerStaminaRecovery"]; ;

            CrouchFactor = (float)RoomManager.I.Settings["PlayerCrouchFactor"]; ;
            CrawlFactor = (float)RoomManager.I.Settings["PlayerCrawlFactor"]; ;

            JumpStrength = (float)RoomManager.I.Settings["PlayerJumpStrength"]; ;
            DriftControl = (float)RoomManager.I.Settings["PlayerDriftControl"]; ;

            ScaleFactor = (float)RoomManager.I.Settings["PlayerScaleFactor"]; ;
            CrouchScaleFactor = (float)RoomManager.I.Settings["PlayerCrouchScaleFactor"]; ;
            CrawlScaleFactor = (float)RoomManager.I.Settings["PlayerCrawlScaleFactor"]; ;

            pv = gameObject.GetComponent<PhotonView>();
            Stats = MultiMenu.HUD.I.Status;
        }
        // Player = PhotonNetwork.Instantiate(Path.Combine("Prefab", "PlayerCharacter"), new Vector3(Random.Range(-2.0f, 2.0f), 0, Random.Range(-2.0f, 2.0f)), Quaternion.identity); ;
        // Start is called before the first frame update
        public override void Start()
        {
            if (!pv.IsMine)
            {
                Destroy(cam.gameObject);
                body.GetComponent<Rigidbody>().isKinematic = true;
                
            }
            base.Start();
            stand();
            if (!PV.IsMine) return;
            eg.Hashtable newHT = new eg.Hashtable();
            newHT["stance"] = 0;
            newHT["falling"] = false;
            newHT["hasjump"] = false;
            newHT["health"] = MaxHealth;
            pv.Owner.SetCustomProperties(newHT);
            pv.RPC("paint", RpcTarget.All);
            defaulthead = cam.transform.localPosition;

        }
        [PunRPC]
        public void paint()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties["Team"] != null)
            {
                Color c = new Color();
                float[] components = (float[])PhotonNetwork.LocalPlayer.CustomProperties["Team"];
                c.r = components[0];
                c.g = components[1];
                c.b = components[2];
                body.GetComponent<Renderer>().material.color = c;
            }
            if (PhotonNetwork.LocalPlayer.CustomProperties["Hat"] != null)
            {
                Color c = new Color();
                float[] components = (float[])PhotonNetwork.LocalPlayer.CustomProperties["Hat"];
                c.r = components[0];
                c.g = components[1];
                c.b = components[2];
                hat.material.color = c;
                hatrim.material.color = c;
            }

        }
        [PunRPC]
        public void paint(float ri, float gi, float bi)
        {
            Color c = new Color();
            c.r = ri;
            c.g = gi;
            c.b = bi;
            body.GetComponent<Renderer>().material.color = c;

        }
        public override void Update()
        {
            if (body.transform.localPosition.y < -100 && PV.IsMine) { Debug.LogError("Under Deathwall"); if (PV.AmOwner && PhotonNetwork.CurrentRoom.PlayerCount > 1) die(); else quitGame(); }
            base.Update();
        }
        [PunRPC]
        public void TakeDamage(float damage, int target, int sender)
        {
            if (pv.Owner.ActorNumber == target // &&!(target==sender) //This adds a self-hit check
                )
            {
                sounds[3].Play();
                //Debug.LogError(health);
                if (!pv.IsMine) return;
                health -= damage;
                if (!(health > 0))
                {
                    Player player = PhotonNetwork.CurrentRoom.GetPlayer(sender);

                    eg.Hashtable newHT = new eg.Hashtable();
                    newHT["score"] = (int)player.CustomProperties["score"] + 1;
                    player.SetCustomProperties(newHT);
                    RoomManager.I.checkWin();
                    die();
                }
            }
        }


        public override void use()
        {
            try
            {
                ((_Object)inventory[CurrentItemIndex]).use();
            }
            catch { };
        }
        public void use2()
        {
            Debug.Log("Use2");
            try
            {
                ((ScopedFireableObject)inventory[CurrentItemIndex]).use2();
            }
            catch { };
        }
        public override void toss() { }
        public void quitGame()
        {
            Debug.LogError("User Quit!");
            Application.Quit(0);
        }

        public void Observe(bool obsSw) {
            if (obsSw)
            {
                cam.transform.localPosition = defaulthead + new Vector3(0, 0, 2);
                cam.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                cam.transform.localPosition = defaulthead;
                cam.transform.localRotation = Quaternion.Euler(0, 0 , 0);
            }
        }
        public void look(float x,float y, float camSens)
        {
            if (!body)  return;
            GameObject player = body.gameObject;
            lastMouse = new Vector3(head.transform.eulerAngles.x + -y * camSens, player.transform.eulerAngles.y + x * camSens, 0);

            head.transform.eulerAngles = lastMouse;
            player.transform.eulerAngles = new Vector3(0, lastMouse.y, 0);
            head.transform.eulerAngles = new Vector3(lastMouse.x, lastMouse.y, 0);
            try
            {
                Stats.text = string.Format(statFString, PhotonNetwork.LocalPlayer.NickName, (int)health, item.clipRounds, item.clipSize, item.totalRounds, PhotonNetwork.LocalPlayer.CustomProperties["score"], -1);
            }

            catch {
                Stats.text = string.Format(statFString, PhotonNetwork.LocalPlayer.NickName, health, 0, 0, 0, PhotonNetwork.LocalPlayer.CustomProperties["score"], -1);
            }
        }
        public void reload()
        {
            ((FireableObject)inventory[CurrentItemIndex]).startReload(false);
        }
        public void move(float horizontalInput, float verticalInput, bool jump,bool sprint)
        {
            float stanceFactor = ((int)pv.Owner.CustomProperties["stance"] == 2 ? CrawlScaleFactor : ((int)pv.Owner.CustomProperties["stance"] == 1 ? CrouchScaleFactor : ScaleFactor));
            Vector3 newVect =
                (((body.transform.forward.normalized) * (verticalInput * MoveSpeed)) +
                ((body.transform.right.normalized) * (horizontalInput * MoveSpeed))) * (sprint ? RunFactor : 1) * stanceFactor;
            body.velocity = new Vector3(
                (body.velocity.y < 1f && body.velocity.y > -1f && !jump ? newVect.x * stanceFactor : (!(bool)pv.Owner.CustomProperties["hasjump"] ?body.velocity.x+(newVect.x*DriftControl): body.velocity.x)),
                (jump && !(bool)pv.Owner.CustomProperties["hasjump"] && body.velocity.y < 0.003f && body.velocity.y > -0.003f ? JumpStrength * stanceFactor : body.velocity.y),
                (body.velocity.y < 1f && body.velocity.y > -1f && !jump ? newVect.z * stanceFactor : (!(bool)pv.Owner.CustomProperties["hasjump"] ? body.velocity.z + (newVect.z * DriftControl) : body.velocity.z))
            );
            pv.Owner.CustomProperties["hasjump"] = jump;
            /*transform.Translate(
                ((transform.worldToLocalMatrix * transform.forward.normalized) * (verticalInput * moveScale)) +
                ((transform.worldToLocalMatrix * transform.right.normalized) * (horizontalInput * moveScale))

                );*/
            stepFreq = (sprint ? stepFreqDefault / 2 : stepFreqDefault);
            if(sprint) try { ((FireableObject)inventory[CurrentItemIndex]).startReload(true); } catch { };



        }
        public override void grab(_Object o)
        {
            if (inventory.Count < MaxInventory && o && PV.IsMine)
            {
                o.PV.TransferOwnership(pv.Owner.ActorNumber);
                o.PV.RPC("snap", RpcTarget.All, head.gameObject.GetComponent<PhotonView>().ViewID,false);
                inventory.Add(o);
                swap(1);
            }
        }
        //spawn at selected spawnpoint
        public override bool spawn(Transform spawnpoint)
        {
            try
            {
                bool baseBool = base.spawn(spawnpoint);
                CurrentItemIndex = -1;
                inventory = new List<_Object>();
                return true && baseBool;
            }
            catch
            {
                return false;
            }
        }
        public void makeSound()
        {
            float translation = UnityEngine.Mathf.Abs(UnityEngine.Mathf.Sqrt(
                ((transform.GetChild(0).position.x - lastMove.x) * (transform.GetChild(0).position.x - lastMove.x)
                ) + (
                (transform.GetChild(0).position.z - lastMove.y) * (transform.GetChild(0).position.z - lastMove.y)
                )));
           // Debug.Log(translation);
            if (translation > stepThresh)
            {
                if (Time.time - lastStep > stepFreq)
                {
                    lastStep = Time.time;
                    sounds[(int)pv.Owner.CustomProperties["stance"]].Play();

                }
            }
            lastMove.x = transform.GetChild(0).position.x;
            lastMove.y = transform.GetChild(0).position.z;
            float drop = transform.GetChild(0).position.y - lastRise;

            eg.Hashtable newHT = new eg.Hashtable();
            //  Debug.Log(drop);
            if (drop < dropThresh&&pv.IsMine) newHT["falling"] = true;
            else if (drop>(dropThresh/2))
            {
                if((bool)pv.Owner.CustomProperties["falling"])sounds[3].Play();
                if(pv.IsMine) newHT["falling"] = false;
            }
            pv.Owner.SetCustomProperties(newHT);
            lastRise = transform.GetChild(0).position.y;

        }
        public override void drop()
        {
            if (inventory.ToArray().Length < 1) return;
            _Object item = ((_Object)inventory[CurrentItemIndex]);
            item.drop();
            item.PV.RPC("snap", RpcTarget.All, -1,true);
            inventory.RemoveAt(CurrentItemIndex);
            item.gameObject.GetComponent<_Object>().PV.TransferOwnership(PhotonNetwork.CurrentRoom.masterClientId);
            item.transform.localScale = new Vector3(1, 1, 1);
            this.swap(-1);

        }
        public override bool die()
        {
            try
            {
                Stats.text = "";
                for (int indexi = 0; indexi < inventory.ToArray().Length; indexi++)
                {
                    Debug.Log("Dropping " + ((_Object)inventory[CurrentItemIndex]).g.name);
                    drop();
                }
                PlayerManager.I.GetComponent<PlayerControl>().Die();
                return base.die() && true;
            }
            catch
            {
                return false;
            }
        }
        public void stand()
        {
            if (g == null) return;
            body.transform.localPosition = body.transform.localPosition + (body.transform.up * 0.2f);
            body.transform.localScale = new Vector3(ScaleFactor-(ScaleFactor/10), ScaleFactor, ScaleFactor);
            hand.transform.localScale = new Vector3(1/ (ScaleFactor - (ScaleFactor / 10)), 1/ScaleFactor, 1/ScaleFactor);
            eg.Hashtable newHT = new eg.Hashtable();
            newHT["stance"] = 0;
            pv.Owner.SetCustomProperties(newHT);
        }
        public void crouch()
        {
            if (g == null) return;
            body.transform.localPosition = body.transform.localPosition + (body.transform.up * 0.1f);
            body.transform.localScale = new Vector3(ScaleFactor - (ScaleFactor / 10), ScaleFactor * CrouchScaleFactor, ScaleFactor);
            hand.transform.localScale = new Vector3(1/ (ScaleFactor - (ScaleFactor / 10)), 1/(ScaleFactor * CrouchScaleFactor), 1/ScaleFactor);
            eg.Hashtable newHT = new eg.Hashtable();
            newHT["stance"] = 1;
            pv.Owner.SetCustomProperties(newHT);
        }
        public void crawl()
        {
            if (g == null) return;
            body.transform.localPosition = body.transform.localPosition + (body.transform.up * 0.05f);
            body.transform.localScale = new Vector3((ScaleFactor - (ScaleFactor / 10)), ScaleFactor * CrawlScaleFactor, ScaleFactor);
            hand.transform.localScale = new Vector3(1/ (ScaleFactor - (ScaleFactor / 10)), 1/(ScaleFactor * CrawlScaleFactor), 1/ScaleFactor);
            eg.Hashtable newHT = new eg.Hashtable();
            newHT["stance"] = 2;
            pv.Owner.SetCustomProperties(newHT);
        }

        public bool swap(int rotAmt)
        {
            if (g == null) return false;
            if (inventory.Count > 0)
            {
                int oldI = CurrentItemIndex;
                CurrentItemIndex = (CurrentItemIndex + inventory.ToArray().Length + rotAmt) % inventory.ToArray().Length;
                _Object current = null;
                try { current = hand.GetComponentInChildren<_Object>(); } catch { }
                if (current != null) current.PV.RPC("snap", RpcTarget.All, -1, false);
                _Object picked = null;
                try { picked = ((_Object)inventory[CurrentItemIndex]); } catch { }
                if (picked != null) picked.PV.RPC("snap", RpcTarget.All, head.gameObject.GetComponent<PhotonView>().ViewID, true);
                try { item = ((FireableObject)inventory[CurrentItemIndex]); } catch { }
                return true;
            }
            else
            {
                CurrentItemIndex = -1;
                item = null;
                return false;
            }
        }

        public void FixedUpdate()
        {

            makeSound();
        }

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            //throw new System.NotImplementedException();
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            //throw new System.NotImplementedException();
        }
    }
}