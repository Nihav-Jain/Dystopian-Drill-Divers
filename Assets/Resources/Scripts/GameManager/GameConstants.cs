using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Environment;

namespace gameManager
{
	public enum EarthType
	{
		SOFT_MUD,
		HARD_MUD,
		GRAVEL,
		CORE,
		CORE_ALPHA
	}

	public enum BuildType
	{
		DEVELOPMENT,
		TEST,
		RELEASE
	}

	public class GameConstants
	{
		public static Vector2 PlayerSpeedVector = new Vector2(0, -1);
		public static float PlayerMaxAngle = 30;
		public static float PlayerRotationStep = 1f;

		public static float PlayerMinSpeed = 1f;
		public static float PlayerMaxSpeed = 4.0f;

		public static Dictionary<EarthType, Earth> EarthLayers = new Dictionary<EarthType, Earth>()
		{
			{EarthType.SOFT_MUD, new Earth("Textures/Strata1Texture", 100)},
			{EarthType.HARD_MUD, new Earth("Textures/Strata2Texture", 200)},
			{EarthType.GRAVEL , new Earth("Textures/Strata3Texture", 500)},
			{EarthType.CORE, new Earth("Textures/core_tile", 1000)},
			{EarthType.CORE_ALPHA, new Earth("Textures/core_tile_alpha", 1000)}
		};

		public const int TotalDepth = 100;
		public const int RandomSnapValue = 20;

		public const int AltitudeFactor = 10;

		public const string DeadBotPositionPref = "DeadBotPosition";

		public const BuildType Build = BuildType.RELEASE;
	}
}