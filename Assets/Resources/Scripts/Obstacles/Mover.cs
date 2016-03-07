using UnityEngine;
using System.Collections;
using player;
using gameManager;

public class Mover : MonoBehaviour {
        
    void Update () {
        if (transform.position.y > 6f)
        {
            Destroy(gameObject);
        }
	}
}
