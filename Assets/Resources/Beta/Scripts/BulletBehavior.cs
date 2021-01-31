using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeanSupreme.v1
{
    public class BulletBehavior : MonoBehaviour
    {
        public float bulletSpeed = 0.5f;
        public float bulletDrop = 0.999f;
        public float bornTime = 0;
        public float lifeTime = 5;
        // Start is called before the first frame update
        void Start()
        {
            bornTime = Time.time;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.GetChild(0).GetComponent<BulletColliderBehavior>().speed = bulletSpeed;
            transform.Translate(transform.worldToLocalMatrix * transform.forward.normalized * bulletSpeed);
            if (Time.time - bornTime > lifeTime)
                Destroy(gameObject);
            bulletSpeed = bulletSpeed * bulletDrop;
        }
    }
}