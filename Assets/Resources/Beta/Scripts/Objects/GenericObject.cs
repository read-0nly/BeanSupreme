using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BeanSupreme.v1
{
    public class GenericObject : _Object
    {
        bool triggered = false;
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
            base.use();
            this.makePhysical(true, true);
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.localPosition = new Vector3(0, 0, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        public override void use2()
        {
            base.use();
            this.makePhysical(false, true);
            this.transform.localPosition = new Vector3(0, 0, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        public override void drop()
        {
            triggered = false;
            base.drop();
        }
    }
}