using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiMenu
{
    public class HUD : MonoBehaviour

    {
        public static HUD I;
        public UnityEngine.UI.Text Status;
        // Start is called before the first frame update
        void Awake()
        {

            DontDestroyOnLoad(gameObject);
            I = this;

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}