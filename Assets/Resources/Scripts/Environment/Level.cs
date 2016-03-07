using UnityEngine;
using System.Collections;
using gameManager;

namespace Environment
{
	public class Level
	{
		public EarthType LevelStrataType { get; private set; }
		public int StrataDepth { get; private set; }

		public GameObject quad { get; private set; }
		private Material strataMaterial;
		private Vector2 materialOffset;

		private Vector3 startPosition;
		private Vector3 changeInPosition;

		public bool isLast;

		public Level(EarthType strataType, int strataDepth, GameObject container, Transform previousTransform)
		{
			LevelStrataType = strataType;
			StrataDepth = strataDepth;

			quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			strataMaterial = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
			strataMaterial.mainTexture = Resources.Load(GameConstants.EarthLayers[strataType].mMaterialName) as Texture;

			strataMaterial.mainTextureScale = new Vector2(1, (strataType == EarthType.CORE) ? 1 : strataDepth * 0.6f);
			quad.GetComponent<Renderer>().material = strataMaterial;
			materialOffset = strataMaterial.GetTextureOffset("_MainTex");
			quad.transform.SetParent(container.transform);

			float quadHeight = Camera.main.orthographicSize * 2.0f;
			float quadWidth = quadHeight * Screen.width / Screen.height;
			float scale = (Screen.height / 2.0f) / Camera.main.orthographicSize;
			quad.transform.localScale = new Vector3(quadWidth, StrataDepth, 1);

			if(previousTransform == null)
			{
				quad.transform.localPosition = new Vector3(0, -StrataDepth / 2.0f, 1);
			}
			else
			{
				quad.transform.localPosition = new Vector3(0, (previousTransform.localPosition.y - previousTransform.localScale.y / 2 - StrataDepth / 2), 1);
			}

			startPosition = quad.transform.localPosition;
			isLast = false;
		}

		public void Update(Vector2 velocity, float speed, float deltaTime)
		{
			if(isLast && quad.transform.localPosition.y > (quad.transform.localScale.y / 2 - 5))
			{
				quad.transform.localPosition = new Vector3(quad.transform.localPosition.x, quad.transform.localScale.y / 2 - 5, 0);
			}
			else
			{
				float newPosition = -velocity.y * speed * deltaTime;
				quad.transform.localPosition = quad.transform.localPosition + Vector3.up * newPosition;

				//velocity.y = 0;
				//materialOffset += (velocity * speed) * deltaTime;
				//strataMaterial.SetTextureOffset("_MainTex", materialOffset);
			}
		}

		public GameObject drawTrail(Transform playerTransform)
		{
			GameObject trail = new GameObject();
			SpriteRenderer spriteRenderer = trail.AddComponent<SpriteRenderer>();

			Sprite trailSprite = Resources.Load<Sprite>("Textures/groovePath");
			spriteRenderer.sprite = trailSprite;
			trail.transform.SetParent(quad.transform);
			//float newY = quad.transform.localScale.y / 4 - (5 - playerTransform.position.y); // -(5 - playerTransform.position.y);
			trail.transform.position = playerTransform.position;
			//trail.transform.eulerAngles = playerTransform.eulerAngles;
			//Debug.Log("Spawned at : " + quad.transform.localPosition.y + trail.transform.localPosition.y);
			return trail;
		}
	}
}