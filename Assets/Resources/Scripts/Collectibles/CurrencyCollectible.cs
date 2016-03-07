using UnityEngine;
using System.Collections;
using gameManager;
using Shop;

namespace Collectibles
{
	public class CurrencyCollectible : Collectible
	{
        public CollectibleGUI collectibleGUI;
		public int value;
        void OnCollisionEnter2D(Collision2D col)
		{
			if (col.gameObject.tag == "Player")
			{
                Instantiate(collectibleGUI, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.identity);
				//GameManager.instance.UpdateCurrency(value);
				GameManager.instance.UpdateCurrency(PlayerPrefs.GetInt(ShopManager.VALUE_PREFIX + ShopItem.GemIncrease, 1));
				Destroy(gameObject);
			}
		}

	}

}