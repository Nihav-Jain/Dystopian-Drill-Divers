using UnityEngine;
using System.Collections;
using player;

namespace Collectibles
{
	public class Collectible : MonoBehaviour
	{
		private PlayerController playerController;

		void Start()
		{
			//playerController = GameObject.Find("Player").GetComponent<PlayerController>();
			gameObject.AddComponent<Mover>();
		}

		void Update()
		{
			//Vector2 velocity = playerController.playerSpeed;
			//float speed = playerController.currenSpeedMagnitude;

			//float newPosition = speed * Time.deltaTime;
			//transform.position = transform.position + (Vector3.up + new Vector3(0, velocity.y * speed * Time.deltaTime, 0f)) * newPosition;
		}
	}
}
