using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using touchPad;
using gameManager;

namespace Shop
{
	public class ShopTouchButton : TouchButton
	{
		[SerializeField]
		public ShopItem shopItem;

		public ShopItemVals shopItemVals {get; private set;}

		[SerializeField]
		private Text CostText;

		void Awake()
		{
			touched = false;
		}

		void Start()
		{
			shopItemVals = gameObject.GetComponent<ShopItemVals>();
			if(GameConstants.Build == BuildType.DEVELOPMENT || !ShopManager.instance.hasKey(shopItem))
			{

				ShopManager.instance.setItemCount(shopItem, 0);
				if (shopItemVals.itemCost.Length > 0)
					ShopManager.instance.setItemCost(shopItem, shopItemVals.itemCost[0]);
				else
					ShopManager.instance.setItemCost(shopItem, 0);
			}
			if(ShopManager.instance.getItemCount(shopItem) >= shopItemVals.itemCost.Length || ShopManager.instance.currency < ShopManager.instance.getItemCost(shopItem))
			{
				gameObject.GetComponent<Button>().interactable = false;
			}
            CostText.text = "X " + ShopManager.instance.getItemCost(shopItem);
			gameObject.GetComponent<Button>().onClick.AddListener(() => { buttonClicked(); });

			for(int i=0;i <ShopManager.instance.getItemCount(shopItem);i++)
			{
				Image img = shopItemVals.itemImage[i];
				if(img != null)
				{
					img.color = new Color(0, 0, 0, 1f);
				}
				else
				{
					img.color = new Color(0, 0, 0, 0f);
				}
			}
			for(int i=ShopManager.instance.getItemCount(shopItem);i<shopItemVals.itemImage.Length;i++)
			{
				Image img = shopItemVals.itemImage[i];
				if (img != null)
				{
					img.color = new Color(0, 0, 0, 0.1f);
				}
			}
        }

		public override void OnPointerDown(PointerEventData data)
		{
			if (!touched)
			{
				touched = true;
				pointerID = data.pointerId;
                //Debug.Log(buttonShopItem +  " button touched");
			}

		}

		public void buttonClicked()
		{
			bool returnVal = ShopManager.instance.buyItem(shopItem, this);
			Debug.Log(returnVal);
			gameObject.GetComponent<Button>().interactable = returnVal;
		}

		public virtual int nextCost(int currentCost)
		{
			// do image effect here
			int index = ShopManager.instance.getItemCount(shopItem);
			//if(index < 0)
			//	index = 0;
			if (index >=1 && index <= shopItemVals.itemCost.Length)
			{
				Image img = shopItemVals.itemImage[index - 1];
				if (img != null)
				{
					img.color = new Color(0, 0, 0, 1f);
				}
			}

			if (index >= shopItemVals.itemCost.Length)
				index = shopItemVals.itemCost.Length - 1;
			int newCost = shopItemVals.itemCost[index];


			CostText.text = "X " + newCost;
			Debug.Log("new cost  = " + newCost);
			return newCost;
		}
	}
}
