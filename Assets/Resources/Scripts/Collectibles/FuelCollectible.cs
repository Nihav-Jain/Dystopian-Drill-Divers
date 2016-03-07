using UnityEngine;
using System.Collections;
using gameManager;
using player;

namespace Collectibles
{
	public class FuelCollectible : Collectible
	{
		void OnCollisionEnter2D(Collision2D col)
		{
			if (col.gameObject.tag == "Player")
			{
				Destroy(gameObject);
				GameManager.playerController.gameObject.GetComponent<PlayerManager>().UpdateFuel(10);
			}
		}

	}

}