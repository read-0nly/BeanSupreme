using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace MultiMenu
{
    public class ChatHUD : MonoBehaviourPunCallbacks, IChatClientListener
    {
        public static ChatHUD I;
        public bool showInput = false;
        public bool show = true;
        public InputField InputBox;
        public TMPro.TMP_Text chatOutput;
        private string[] chatHistory = new string[10];
        private ChatClient cClient;
        private static string chatID = "bafeb3c2-6d6d-4988-ac48-cc365b49e4c0";
        private static string chatVersion = "1.0a";
        public string Nickname = "Bean";
        public string Region = "us";
        public string Channel = "General";
        public bool reconnectNow = false;
        public CursorLockMode oldLock = CursorLockMode.Locked;

        // Start is called before the first frame update
        void Start()
        {
            I = this;
            cClient = new ChatClient(this);
            reconnect();
        }

        // Update is called once per frame
        void Update()
        {
            cClient.Service();
            if (Input.GetButton("Chat") && !showInput && cClient.CanChat)
            {
                showInput = true;
                if (!cClient.CanChatInChannel(Channel)) cClient.Subscribe(Channel);
                oldLock = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;
                InputBox.gameObject.SetActive(showInput);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(InputBox.gameObject, null);
                InputBox.Select();
                InputBox.ActivateInputField();

            }
            else if (Input.GetButton("Submit") && showInput && cClient.CanChat)
            {
                Debug.Log(InputBox.isFocused);
                showInput = false;
                string message = InputBox.text;
                bool sendResult = (message != "" ? cClient.PublishMessage(Channel, message) : false);
                if (sendResult)
                {
                    InputBox.text = "";
                    InputBox.gameObject.SetActive(showInput);
                    Cursor.lockState = oldLock;
                    oldLock = CursorLockMode.Locked;
                }
                else
                {
                    rotateMessages(1);
                    chatHistory[chatHistory.Length - 1] = "Send Failed!";
                    printChat();
                    InputBox.gameObject.SetActive(showInput);
                    Cursor.lockState = oldLock;
                    oldLock = CursorLockMode.Locked;

                }

            }
        }
        private void FixedUpdate()
        {
        }

        public void connect()
        {
            cClient.ChatRegion = this.Region;
            cClient.Connect(chatID, chatVersion, new AuthenticationValues(Nickname));
            reconnectNow = false;
        }
        public void reconnect()
        {
            reconnectNow = true;
            try
            {
                if (!(cClient.State == ChatState.Disconnected || cClient.State == ChatState.Uninitialized))
                {
                    cClient.Disconnect();
                }
                else
                {
                    connect();
                }
            }
            catch { }

        }
        public void reconnect(string region, string username)
        {
            this.Nickname = username;
            this.Region = region;
            reconnect();

        }
        public void rotateMessages(int i)
        {
            string[] bucket = new string[chatHistory.Length];
            if (i > 0)
            {
                for (int j = 0; j < (chatHistory.Length - i); j++)
                {
                    bucket[j] = chatHistory[j + i];
                }
            }
            else
            {
                for (int j = 0; j < (chatHistory.Length + i); j++)
                {
                    bucket[j - i] = chatHistory[j];
                }

            }
            chatHistory = bucket;
        }
        public void printChat()
        {
            chatOutput.text = "";
            foreach (string s in chatHistory)
            {
                chatOutput.text += s + "\r\n";
            }
        }


        public void DebugReturn(DebugLevel level, string message)
        {
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = (level == DebugLevel.ERROR ? string.Format("[<color=#F00>{0}</color>] : {1}", level.ToString(), message) :
                    (level == DebugLevel.WARNING ? string.Format("[<color=#FF0>{0}</color>] : {1}", level.ToString(), message) :
                        string.Format("[<color=#888>{0}</color>] : {1}", level.ToString(), message)
                    )
                );
            printChat();
        }

        public void OnDisconnected()
        {
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = "Disconnected!";
            if (reconnectNow)
            {
                connect();
            }
            printChat();
        }

        public void OnChatStateChange(ChatState state)
        {
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = "New state: " + state.ToString();
            printChat();
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                rotateMessages(1);
                chatHistory[chatHistory.Length - 1] = string.Format("<b><color=#0FF>{0}</color> <color=#0AA><{1}></color></b>: {2}", senders[i], channelName, messages[i]);
            }
            printChat();

        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = string.Format("<i><b><color=#0FF>{0}</color> <color=#0AA><{1}></color></b>: {2}</i>", sender, channelName, message);
            printChat();
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            /*
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = "<color=#888>Subscribed to: ";
            for(int i = 0; i < channels.Length; i++)
            {
                chatHistory[chatHistory.Length - 1] += (results[i]?channels[i]+"; ":"") ;
            }
            chatHistory[chatHistory.Length - 1] += "</color>";
            printChat();
            */
        }

        public void OnUnsubscribed(string[] channels)
        {
            /*
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = "<color=#888>Unsubscribed from: ";
            for (int i = 0; i < channels.Length; i++)
            {
                chatHistory[chatHistory.Length - 1] +=channels[i] + "; ";
            }
            chatHistory[chatHistory.Length - 1] += "</color>";
            printChat();
            */
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = string.Format("{0} is now {1}: {2}", user, status, (gotMessage ? message : ""));
            printChat();
        }

        public void OnUserSubscribed(string channel, string user)
        {
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = string.Format("<color=#888>{0} joined {1}</color>", user, channel);
            printChat();
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            rotateMessages(1);
            chatHistory[chatHistory.Length - 1] = string.Format("<color=#888>{0} joined {1}</color>", user, channel);
            printChat();
        }
    }
}