using UnityEngine;
using System.Collections;
using touchPad;
using gameManager;

namespace player
{
    [System.Serializable]
    public class Boundary
    {
        public float XMin;
        public float XMax;
        public float YMin;
        public float YMax;
    }


    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("speed player moves")]
        public float Speed;

        [SerializeField]
        [Tooltip("set the bounds that the player can move between")]
        private Boundary Boundary;

        [SerializeField]
        [Range(0,1)]
        [Tooltip("How much the player has to move the phone before movement is registered")]
        private float DeadZoneDelta;

        [SerializeField]
        [Tooltip("modifies how much rotation occurs")]
        private float Tilt;

        [SerializeField]
        [Tooltip("The maximum amount of rotation from InitRotation that can occur")]
        private float MaxRotationOffset;

        [SerializeField]
        [Tooltip("Neutral rotation for the player")]
        private float InitRotation;

        
        private Quaternion calibrationQuaternion;

        public float OffScreenPosition;
        public float playerDefaultPosition;
        private Vector2 MoveVelocity;
        public float DampTimeOffScreen;
        public bool CanMovePlayerOffScreen;
        public bool CanMovePlayerBack;
        public float DampTimeMoveBack;

        public Vector2 playerSpeed { get; private set; }		// this is actually speed of the scrolling background
		public float currenSpeedMagnitude { get; set; }
        public float skyScrollSpeed;

		public Vector2 materialOffset {get; private set;}		// offset of the scrolling, repeated texture material - this is what gives makes the background scroll
		private Material backgroundMaterial;

		private PlayerManager playerManager;
		public float distanceTravelled { get; private set; }
		private GameObject trail;

        public bool CanPlayerMove { get; set; }

        // Use this for initialization
        void Awake()
        {
			InitRotation = 0;
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.rotation = InitRotation;
            CalibrateAccellerometer();

			playerSpeed = GameConstants.PlayerSpeedVector;

			playerManager = gameObject.GetComponent<PlayerManager>();
			distanceTravelled = 0;

            CanPlayerMove = true;

            playerDefaultPosition = transform.position.y;

            //trail = GameObject.Find("TrailTexture");
        }

        void FixedUpdate()
        {

            if (CanPlayerMove)
            {
                
                //move player left/right
                Vector3 accelerationRaw = Input.acceleration;
                Vector3 acceleration = FixAcceleration(accelerationRaw);
                if(Mathf.Abs(acceleration.x) < DeadZoneDelta)
                {
                    acceleration.x = 0.0f;
                }
                Vector2 movement = new Vector2(acceleration.x, 0.0f);

                Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
                rigidbody2D.constraints = RigidbodyConstraints2D.None;
                rigidbody2D.velocity = movement * Speed;

                rigidbody2D.position = new Vector2
                (
                    Mathf.Clamp(rigidbody2D.position.x, Boundary.XMin, Boundary.XMax),
                    Mathf.Clamp(rigidbody2D.position.y, Boundary.YMin, Boundary.YMax)
                );

                //rotate player
                rigidbody2D.rotation = Mathf.Clamp(InitRotation - (rigidbody2D.velocity.x * -Tilt), InitRotation - MaxRotationOffset, InitRotation + MaxRotationOffset);
                playerSpeed = new Vector2(Mathf.Cos((90 - rigidbody2D.rotation) * Mathf.Deg2Rad), -Mathf.Sin(Mathf.Deg2Rad * (90 - rigidbody2D.rotation)));
            }
            else
            {
                Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            if (CanMovePlayerOffScreen)
            {

                MovePlayerOffScreen();
                if (transform.position.y <= OffScreenPosition)
                {
                    //Debug.Log("done moving player offscreen");
                    CanMovePlayerOffScreen = false;
                    CanMovePlayerBack = true;
                }
            }

            if (CanMovePlayerBack)
            {
                MovePlayerBackToPosition();
                //Debug.Log(transform.position.y);
                if (transform.position.y >= Boundary.YMin - 0.3)
                {
                    //Debug.Log("player back to original position");
                    CanMovePlayerBack = false;
                }
            }


            //trail.transform.localPosition = gameObject.transform.localPosition;
            //trail.transform.localRotation = gameObject.transform.localRotation;
        }

        // Update is called once per frame
        void Update()
        {

            if (!GameManager.instance.isInEarth)
            {
                currenSpeedMagnitude = skyScrollSpeed;
            }
            else
            {
                currenSpeedMagnitude = Mathf.Lerp(GameConstants.PlayerMinSpeed, GameConstants.PlayerMaxSpeed, (playerManager.GetCurrentRPMs() - playerManager.GetRpmMin()) / (playerManager.GetRpmMax() - playerManager.GetRpmMin()));
                //backgroundMaterial.SetTextureOffset("_MainTex", materialOffset);
            }

            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();		// refactor
			distanceTravelled += currenSpeedMagnitude * Mathf.Sin(Mathf.Deg2Rad * rigidbody2D.rotation) * Time.deltaTime;
			//Debug.Log(distanceTravelled);
		}

        public void CalibrateAccellerometer()
        {
            Vector3 accelerationSnapshot = Input.acceleration;
            Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
            calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
        }

        Vector3 FixAcceleration(Vector3 acceleration)
        {
            Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
            return fixedAcceleration;
        }

        public void MovePlayerOffScreen()
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            transform.position = Vector2.SmoothDamp(transform.position, new Vector2(transform.position.x, OffScreenPosition), ref MoveVelocity, DampTimeOffScreen);
            rigidbody2D.position = new Vector2
                (
                    Mathf.Clamp(rigidbody2D.position.x, Boundary.XMin, Boundary.XMax),
                    rigidbody2D.position.y
                );

        }

        public void MovePlayerBackToPosition()
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            transform.position = Vector2.SmoothDamp(transform.position, new Vector2(transform.position.x, playerDefaultPosition), ref MoveVelocity, DampTimeMoveBack);
            rigidbody2D.position = new Vector2
                (
                    Mathf.Clamp(rigidbody2D.position.x, Boundary.XMin, Boundary.XMax),
                    rigidbody2D.position.y
                );
        }
    }
}
