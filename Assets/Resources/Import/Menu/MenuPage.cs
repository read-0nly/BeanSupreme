using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace MultiMenu
{
    public class MenuPage : MonoBehaviourPunCallbacks
    {
        public MenuPage This;
        public virtual void refresh() { }
        public virtual void submit() { }
        public virtual void cancel() { Menu.instance.setMenu("MainMenu"); }
        public virtual string[] getInput()
        {
            string[] result = new string[0];
            return result;
        }
    }
}