using UnityEngine;
using System.Collections;

namespace gameManager
{
	public class Spawner : MonoBehaviour
	{

		[SerializeField]
		public GameObject[] spawnerPrefabs;

		[SerializeField]
		public bool isSpawnerActive = true;

		public GameObject Container;
		public Vector2 spawnValues;
		public int spawnCount;
		public float spawnWait;
		public float startWait;
		public float waveWait;

		public bool SpawnStarted;

		public bool waitForDestroy;

		void Start()
		{
			Container = GameObject.Find("EarthLevels");
			SpawnStarted = false;
			waitForDestroy = false;
		}

		void Update()
		{

			if (GameManager.instance.StartSpawners && !SpawnStarted)
			{
				SpawnStarted = true;
				StartCoroutine(SpawnWaves());
			}
		}

		protected virtual IEnumerator SpawnWaves()
		{
			yield return new WaitForSeconds(startWait);

			while (true)
			{
				if(!waitForDestroy)
				{
					for (int i = 0; i < spawnCount; i++)
					{
						Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, 0);
						Quaternion spawnRotation = Quaternion.identity;

						GameObject hazard = (GameObject)Instantiate(spawnerPrefabs[Random.Range(0, spawnerPrefabs.Length)], new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y), spawnRotation);
						hazard.transform.SetParent(Container.transform);
						yield return new WaitForSeconds(spawnWait);
					}
				}
				yield return new WaitForSeconds(waveWait);
			}

		}
	}
}
