using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Shop
{

	public enum ShopItem
	{
		FuelCapacity,
		GemIncrease,
		RPMIncrease,
		OverHeat,
		Phasers
	}

	public class ShopManager : MonoBehaviour
	{
		[SerializeField]
		private Text alertText;
		[SerializeField]
		private int initialCurrency = 20;

        [SerializeField]
        private Text currencyText;

		//public int itemCount;

		public int currency { get; private set; }

		public static ShopManager instance = null;
		public const string COST_PREFIX = "Cost_";
		public const string BOUGHT_PREFIX = "Bought_";
		public const string COUNT_PREFIX = "Count_";
		public const string VALUE_PREFIX = "Value_";
		public const string CURRENCY = "Currency";

		void Awake()
		{
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            // uncomment the nect 2 lines to set the initial curency for demo, then comment it back again or else currency will be reet everytime you start the game
			//PlayerPrefs.DeleteAll();
            //updateCurrency(initialCurrency);
            currency = PlayerPrefs.GetInt(CURRENCY, 3000);
            currencyText.text = "Gem Count: " + currency;
		}

		void Start()
		{
		}

		public bool buyItem(ShopItem item, ShopTouchButton caller)
		{
			int itemCost = PlayerPrefs.GetInt(COST_PREFIX + item.ToString(), -1);
			bool returnVal = true;
			if (getItemCount(item) >= caller.shopItemVals.itemCost.Length)
				return false;
			if(currency >= itemCost)
			{
				setItemBought(item);
				setItemValue(item, caller.shopItemVals.itemValue[getItemCount(item)]);
				setItemCount(item, getItemCount(item) + 1);
				updateCurrency(currency - itemCost);
				alertText.text = "Upgrade bought";
				itemCost = caller.nextCost(itemCost);
				setItemCost(item, itemCost);
                Debug.Log(getItemCount(item));

				if (getItemCount(item) >= caller.shopItemVals.itemCost.Length || currency < itemCost)
					returnVal = false;
			}
			else
			{
				alertText.text = "Insufficient Funds";
			}
			//Debug.Log(this);
			return returnVal;
		}

		public bool isItemBought(ShopItem item)
		{
			int itemBuyStatus = PlayerPrefs.GetInt(BOUGHT_PREFIX + item.ToString(), 0);
			return (itemBuyStatus == 1) ? true : false;
		}

		public void setItemBought(ShopItem item)
		{
			PlayerPrefs.SetInt(BOUGHT_PREFIX + item.ToString(), 1);
			PlayerPrefs.Save();
		}

		public void setItemCost(ShopItem item, int cost)
		{
			PlayerPrefs.SetInt(COST_PREFIX + item.ToString(), cost);
			PlayerPrefs.Save();
		}

		public int getItemCost(ShopItem item)
		{
			return PlayerPrefs.GetInt(COST_PREFIX + item.ToString(), -1);
		}

		public void setItemCount(ShopItem item, int count)
		{
			PlayerPrefs.SetInt(COUNT_PREFIX + item.ToString(), count);
			PlayerPrefs.Save();
		}

		public int getItemCount(ShopItem item)
		{
			return PlayerPrefs.GetInt(COUNT_PREFIX + item.ToString(), 0);
		}

		public void setItemValue(ShopItem item, int value)
		{
			PlayerPrefs.SetInt(VALUE_PREFIX + item.ToString(), value);
			PlayerPrefs.Save();
		}

		public int getItemValue(ShopItem item)
		{
			return PlayerPrefs.GetInt(VALUE_PREFIX + item.ToString(), 0);
		}

		private void updateCurrency(int newCurrency)
		{
			currency = newCurrency;
            currencyText.text = "Gem Count: " + currency;
            PlayerPrefs.SetInt(CURRENCY, currency);
			PlayerPrefs.Save();
		}

		public bool hasKey(ShopItem item)
		{
			return PlayerPrefs.HasKey(COST_PREFIX + item.ToString()) && PlayerPrefs.HasKey(COUNT_PREFIX + item.ToString());
		}

		public override string ToString()
		{
			string str = "ShopManager:\n";
			Array enumList = Enum.GetValues(typeof(ShopItem));
			foreach(ShopItem item in enumList)
			{
				str += string.Format("{0}: Cost={1}, Count={2}\n", item, getItemCost(item), getItemCount(item));
			}
			return str;
		}

		public void LoadGame()
		{
			Application.LoadLevel(1);
		}

		public void LoadMenu()
		{
			Application.LoadLevel(0);
		}
	}
}
