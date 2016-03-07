using UnityEngine;
using System.Collections;
using Collectibles;
using gameManager;

public class DiamondMineBonus : MonoBehaviour 
{
	public CollectibleGUI collectibleGUI;
	public int value;

	private int i;
	//void OnDestroy()
	//{
	//	GameManager.instance.UpdateCurrency(value);
	//	i = 0;
	//	InvokeRepeating("instantiateDiamondGUI", 0.01f, 0.5f);
	//	//StartCoroutine(spawnDiamondGUI());
	//	Debug.Log("diamond mine destroyed");
	//}

	public void instantiateDiamondGUI()
	{
		//Debug.Log(i);
		Instantiate(collectibleGUI, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.identity);
		GameManager.instance.UpdateCurrency(value);
		//i++;
		//if (i >= value / 10)
		//	CancelInvoke();
	}

	private IEnumerator spawnDiamondGUI()
	{
		for (int i = 0; i < value; i++)
		{
			GameObject gui = GameObject.Instantiate(Resources.Load("Prefabs/DiamondGUI", typeof(GameObject)), new Vector3(transform.position.x, transform.position.y, -2), Quaternion.identity) as GameObject;
			gui.SetActive(true);
			yield return new WaitForSeconds(0.5f);
		}

	}


}
