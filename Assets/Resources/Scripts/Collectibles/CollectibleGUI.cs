using UnityEngine;
using System.Collections;

namespace Collectibles
{
    public class CollectibleGUI : MonoBehaviour {

        public Vector3 target;
        public float speed;
        public float lifetime;
	
        void Start()
        {
            Destroy(gameObject, lifetime);
        }
	    // Update is called once per frame
	    void Update () {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
	    }
    }
}
