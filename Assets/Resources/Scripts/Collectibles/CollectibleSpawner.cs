using UnityEngine;
using System.Collections;
using gameManager;

namespace Collectibles
{
	public class CollectibleSpawner : Spawner
	{
		protected override IEnumerator SpawnWaves()
		{
			yield return new WaitForSeconds(startWait);

			while (true)
			{
				if (!waitForDestroy)
				{
					for (int i = 0; i < spawnCount; i++)
					{
						GameObject objectToSpawn;
						float probability = Random.Range(0.0f, 1.0f);
						//Debug.Log(probability);
						if(probability <= GameManager.instance.fuelSpwnProbability || GameManager.instance.isFuelLow())
						{
							objectToSpawn = spawnerPrefabs[0];
						}
						else
						{
							objectToSpawn = spawnerPrefabs[1];
						}
						//objectToSpawn = spawnerPrefabs[0];	// comment out
						Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, 0);
						Quaternion spawnRotation = Quaternion.identity;

						GameObject hazard = (GameObject)Instantiate(objectToSpawn, new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y), spawnRotation);
						hazard.transform.SetParent(Container.transform);
						yield return new WaitForSeconds(spawnWait);
					}
				}
				yield return new WaitForSeconds(waveWait);
			}
		}
	}
}
