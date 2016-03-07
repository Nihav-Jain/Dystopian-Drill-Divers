using UnityEngine;
using System.Collections;
using player;
using Environment;
using gameManager;

public class SpawnHazards : Spawner {

	protected override IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds(startWait);

		while (true)
		{
			if(!waitForDestroy)
			{
				GameObject obstacleToSpawn;
				int probability = Random.Range(0, GameManager.instance.MaxLevelNumber);
				if (probability <= GameManager.instance.LevelNumber)
				{
					obstacleToSpawn = spawnerPrefabs[1];
				}
				else
				{
					obstacleToSpawn = spawnerPrefabs[0];
				}

				Obstacle obstacleProps = obstacleToSpawn.GetComponent<Obstacle>();

				for (int i = 0; i < obstacleProps.spawnCount; i++)
				{
					Vector3 spawnPosition = new Vector3(Random.Range(-obstacleProps.spawnValues.x, obstacleProps.spawnValues.x), obstacleProps.spawnValues.y, 0);
					Quaternion spawnRotation = Quaternion.identity;

					GameObject hazard = (GameObject)Instantiate(obstacleToSpawn, spawnPosition, spawnRotation);
					hazard.transform.SetParent(Container.transform);
					yield return new WaitForSeconds(obstacleProps.spawnWait);
				}
			}

			yield return new WaitForSeconds(waveWait);
		}
	}

}
