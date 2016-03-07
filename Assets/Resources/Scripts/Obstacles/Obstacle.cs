using UnityEngine;
using System.Collections;

public enum SpawnFrequency
{
	Constant,
	IncreaseWithLevel,
	DecreaseWithLevel
}

public class Obstacle : MonoBehaviour {
	public Vector2 spawnValues;
	public int spawnCount;
	public float spawnWait;
	public float level1SpawnProbability;
	public SpawnFrequency spawnFrequency;
}
