using UnityEngine;
using System.Collections;
using System;
using gameManager;
using player;
using System.Collections.Generic;

namespace Environment
{
	public class EnvironmentBuilder : MonoBehaviour
	{
		public Level[] mEarthLevels { get; private set; }

		private PlayerController playerController;
		private GameObject backgroundContainer;
		public GameObject background { get; private set; }
		private Material backgroundMaterial;
		private Vector2 materialOffset;

		private List<GameObject> trailSprites;

		public bool Scroll { get; set; }
		private float scrollLimit;
		private GameObject sky;

		public bool isInEarth { get; private set; }

        private float offset;
        private float screenHalfHeight = 5f;
		void Start()
		{
			playerController = GameObject.Find("Player").GetComponent<PlayerController>();

			backgroundContainer = GameObject.Find("EarthLevels");
			mEarthLevels = new Level[(2 * GameConstants.TotalDepth / GameConstants.RandomSnapValue) + 1];

			sky = GameObject.Find("Sky");
			Transform prevTransform = sky.transform;

            offset = -sky.transform.localScale.y / 2 + screenHalfHeight - sky.transform.position.y;
            scrollLimit = GameConstants.TotalDepth + 9.5f + sky.transform.localScale.y + offset - playerController.gameObject.transform.position.y - screenHalfHeight*2;
			int i, newDepth = 0, newEarth;
			int numEarthTypes = Enum.GetNames(typeof(EarthType)).Length;
			int strataDivision = mEarthLevels.Length / 3;
			EarthType strataEarthType = EarthType.SOFT_MUD;

			for (i = 0; i < mEarthLevels.Length-1;i++ )
			{
				//if(i%2 == 0)
				//	newDepth = UnityEngine.Random.Range(GameConstants.RandomSnapValue / 4, GameConstants.RandomSnapValue * 3 / 4);
				//else
				//	newDepth = GameConstants.RandomSnapValue - newDepth;
				newEarth = UnityEngine.Random.Range(0, 2 * numEarthTypes);

				if (i < strataDivision)
				{
					strataEarthType = EarthType.SOFT_MUD;
				}
				else if (i < 2 * strataDivision)
					strataEarthType = EarthType.HARD_MUD;
				else
					strataEarthType = EarthType.GRAVEL;
				newDepth = 10;
				mEarthLevels[i] = new Level(strataEarthType, newDepth, backgroundContainer, prevTransform);
				prevTransform = mEarthLevels[i].quad.transform;
			}
			mEarthLevels[i] = new Level(EarthType.CORE, 10, backgroundContainer, prevTransform);
			mEarthLevels[mEarthLevels.Length - 1].isLast = true;
			mEarthLevels[mEarthLevels.Length - 1].quad.transform.position += Vector3.back * 2;

			makeCore(prevTransform);

			trailSprites = new List<GameObject>();

			isInEarth = false;
		}

		void Update()
		{
			if (backgroundContainer.transform.position.y >= (sky.transform.localScale.y + offset - playerController.gameObject.transform.position.y - playerController.gameObject.transform.localScale.y / 2))
					isInEarth = true;
			if(Scroll)
			{
				if(backgroundContainer.transform.position.y >= scrollLimit)
				{
					GameManager.playerController.CanPlayerMove = false;
					Rigidbody2D playerRigidBody = GameManager.playerController.gameObject.GetComponent<Rigidbody2D>();
					playerRigidBody.velocity = Vector2.zero;
					GameManager.instance.HandleGameOver(GameOverState.EndOfLevel);
				}
				else
				{
					float deltaTime = Time.deltaTime;
					float newPosition = -playerController.playerSpeed.y * playerController.currenSpeedMagnitude * deltaTime;
					backgroundContainer.transform.localPosition = backgroundContainer.transform.localPosition + Vector3.up * newPosition;
					if (isInEarth && deltaTime > 0)
					{
						drawTrail();
					}
					int i;
					for (i = trailSprites.Count - 1; i >= 0; i--)
					{
						GameObject trail = trailSprites[i];
						if (trail.transform.position.y > 6)
						{
							trailSprites.Remove(trail);
							GameObject.Destroy(trail);
						}
					}
				}
			}
			//mEarthLevels[0].drawTrail(playerController.gameObject.transform.position);
		}

		void drawTrail()
		{
			GameObject trail = new GameObject();
			SpriteRenderer spriteRenderer = trail.AddComponent<SpriteRenderer>();

			Sprite trailSprite = Resources.Load<Sprite>("Textures/groovePath2");
			spriteRenderer.sprite = trailSprite;
			trail.transform.SetParent(backgroundContainer.transform);
			trail.transform.position = playerController.gameObject.transform.position;
			//trail.transform.eulerAngles = playerController.gameObject.transform.eulerAngles;
			trailSprites.Add(trail);
			//trailSprites.Add(mEarthLevels[0].drawTrail(playerController.gameObject.transform));
			//Debug.Log(trail.transform.eulerAngles);
		}

		private void printPositions()
		{
			foreach(GameObject trail in trailSprites)
			{
				Debug.Log(mEarthLevels[0].quad.transform.localPosition.y + trail.transform.localPosition.y - mEarthLevels[0].quad.transform.position.y);
			}
		}

		public float getAltitude()
		{
			return GameConstants.AltitudeFactor * ((sky.transform.localScale.y + offset - playerController.gameObject.transform.position.y - playerController.gameObject.transform.localScale.y / 2) - backgroundContainer.transform.position.y);
		}

		private void makeCore(Transform prev)
		{

			//prev.position += Vector3.up * 3;

			GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			quad.name = "CoreAlpha";
			Material strataMaterial = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
			strataMaterial.mainTexture = Resources.Load(GameConstants.EarthLayers[EarthType.CORE_ALPHA].mMaterialName) as Texture;

			int strataDepth = 1;
			strataMaterial.mainTextureScale = new Vector2(1, strataDepth * 0.6f);
			quad.GetComponent<Renderer>().material = strataMaterial;
			materialOffset = strataMaterial.GetTextureOffset("_MainTex");
			materialOffset = new Vector2(0, 0.42f);
			strataMaterial.SetTextureOffset("_MainTex", materialOffset);
			quad.transform.SetParent(backgroundContainer.transform);

			float quadHeight = Camera.main.orthographicSize * 2.0f;
			float quadWidth = quadHeight * Screen.width / Screen.height;
			float scale = (Screen.height / 2.0f) / Camera.main.orthographicSize;
			quad.transform.localScale = new Vector3(quadWidth, strataDepth, 1);
			quad.transform.localPosition = new Vector3(0, (prev.localPosition.y - prev.localScale.y / 2 - strataDepth / 2), -1);

		}

	
	}
}
