using UnityEngine;
using System.Collections;

public class CustomCameraScript : MonoBehaviour {

	public Transform target;

	public Camera _camera;
	// Use this for initialization

	public RotationLock spinLock = RotationLock.Free;
	/** How pitch rotation is affected (Free, Limited, Locked) */
	public RotationLock pitchLock = RotationLock.Free;

	public float horizontalOffset = 0f;
	/** The vertical position offset */
	public float verticalOffset = 2f;
	
	/** The normal distance to keep from its target */
	public float distance = 2f;
	/** If True, then the camera will detect Colliders to try to avoid clipping through walls */
	public bool detectCollisions = true;
	/** The distance to keep away from Colliders, if detectCollisions = True */
	public float collisionOffset = 0f;
	/** The minimum distance to keep from its target */
	public float minDistance = 1f;
	/** The maximum distance to keep from its target */
	public float maxDistance = 3f;

	
	/** The speed of spin rotations */
	public float spinSpeed = 5f;
	/** The acceleration of spin rotations */
	public float spinAccleration = 5f;
	/** The deceleration of spin rotations */
	public float spinDeceleration = 5f;

	/** If True, then the pitch rotation will be reset when the camera is made active */
	public bool resetSpinWhenSwitch = false;
	/** The offset in spin (yaw) angle if alwaysBehind = true */
	public float spinOffset = 0f;
	/** The maximum spin angle, if spinLock = RotationLock.Limited */
	public float maxSpin = 40f;
	public float minSpin = -40f;
	
	/** The speed of pitch rotations */
	public float pitchSpeed = 3f;
	/** The acceleration of pitch rotations */
	public float pitchAccleration = 20f;
	/** The deceleration of pitch rotations */
	public float pitchDeceleration = 20f;
	/** The maximum pitch angle, if pitchLock = RotationLock.Limited */
	public float maxPitch = 40f;
	public float minPitch = -40f;
	/** If True, then the pitch rotation will be reset when the camera is made active */
	public bool resetPitchWhenSwitch = false;

	public bool CameraIsFixed = false;

	public float sensitivity = 1;
	
	private float actualCollisionOffset = 0f;
	
	private float deltaDistance = 0f;
	private float deltaSpin = 0f;
	private float deltaPitch = 0f;
	
	private float roll = 0f;
	private float spin = 0f;
	private float pitch = 0f;
	
	private float initialPitch = 0f;
	private float initialSpin = 0f;
	
	private Vector3 centrePosition;
	private Vector3 targetPosition;
	private Quaternion targetRotation;

	private Vector2 inputMovement;


	void Awake ()
	{
		if (GetComponent <Camera>())
		{
			_camera = GetComponent <Camera>();
			Debug.Log("Camera name" + _camera.transform.name);
		}
		initialPitch = transform.eulerAngles.x;
		initialSpin = transform.eulerAngles.y;
	}
	void Start () {
		Vector3 angles = transform.eulerAngles;
		spin = angles.y;
		roll = angles.z; 
		
		UpdateTargets ();
		SnapMovement ();
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SnapMovement ()
	{
		transform.rotation = targetRotation;
		transform.position = targetPosition;
	}

	public void ResetRotation ()
	{
		if (pitchLock != RotationLock.Locked && resetPitchWhenSwitch)
		{
			pitch = initialPitch;
		}
		if (spinLock != RotationLock.Locked && resetSpinWhenSwitch)
		{
			spin = initialSpin;
		}

		if (!CameraIsFixed) {
			UpdateTargets ();
			SnapMovement ();
		}
	}

	private void DetectCollisions ()
	{
		if (detectCollisions && target != null)
		{
			RaycastHit hit;
			if (Physics.Linecast (target.position + new Vector3 (0, verticalOffset, 0f), targetPosition, out hit))
			{
				actualCollisionOffset = (targetPosition - hit.point).magnitude + collisionOffset;
			}
			else
			{
				actualCollisionOffset = 0f;
			}
		}
	}

	private void FixedUpdate ()
	{
		if (CameraIsFixed) {
			return;
		}

		UpdateTargets ();
		DetectCollisions ();

		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * 10f);

		transform.position = Vector3.Lerp (transform.position, targetPosition - (targetPosition - centrePosition).normalized * actualCollisionOffset, Time.deltaTime * 10f);
	}

	void UpdateTargets()
	{
		if (!target)
		{
			return;
		}

		if (Toolbox.Instance.GetDragState () == DragState._Camera && !Toolbox.Instance.isTransitioning())
		{
			inputMovement = Toolbox.Instance.GetDragVector ();
		}
		else
		{
			inputMovement = Vector2.zero;
		}

		inputMovement =  inputMovement * 0.3f;
		if (spinLock != RotationLock.Locked)
		{
			if (inputMovement.x > 60f/sensitivity)
			{
				deltaSpin = Mathf.Lerp (deltaSpin, spinSpeed, spinAccleration * Time.deltaTime * inputMovement.x);
			}
			else if (inputMovement.x < -60f/sensitivity)
			{
				deltaSpin = Mathf.Lerp (deltaSpin, -spinSpeed, spinAccleration * Time.deltaTime * -inputMovement.x);
			}
			else
			{
				deltaSpin = Mathf.Lerp (deltaSpin, 0f, spinDeceleration * Time.deltaTime);
			}
			
			if (spinLock == RotationLock.Limited)
			{
				if (deltaSpin > 0f)
				{
					if (maxSpin - spin < 5f)
					{
						deltaSpin *= (maxSpin - spin) / 5f;
					}
				}
				else if (deltaSpin < 0f)
				{
					if (maxSpin + spin < 5f)
					{
						deltaSpin *= (maxSpin + spin) / 5f;
					}
				}
			}
			
			spin += deltaSpin;
			
			if (spinLock == RotationLock.Limited)
			{
				spin = Mathf.Clamp (spin, minSpin, maxSpin);
			}
		}
				
				
		if (pitchLock != RotationLock.Locked)
		{
			if (inputMovement.y > 60f/sensitivity)
			{
				deltaPitch = Mathf.Lerp (deltaPitch, pitchSpeed, pitchAccleration * Time.deltaTime * inputMovement.y);
			}
			else if (inputMovement.y < -60f/sensitivity)
			{
				deltaPitch = Mathf.Lerp (deltaPitch, -pitchSpeed, pitchAccleration * Time.deltaTime * -inputMovement.y);
			}
			else
			{
				deltaPitch = Mathf.Lerp (deltaPitch, 0f, pitchDeceleration * Time.deltaTime);
			}
			
			if (pitchLock == RotationLock.Limited)
			{
				if (deltaPitch > 0f)
				{
					if (maxPitch - pitch < 5f)
					{
						deltaPitch *= (maxPitch - pitch) / 5f;
					}
				}
				else if (deltaPitch < 0f)
				{
					if (maxPitch + pitch < 5f)
					{
						deltaPitch *= (maxPitch + pitch) / 5f;
					}
				}
			}
			
			pitch += deltaPitch;
			
			if (pitchLock == RotationLock.Limited)
			{
				pitch = Mathf.Clamp (pitch, minPitch, maxPitch);
			}
		}
	
		if (pitchLock == RotationLock.Locked)
		{
			pitch = maxPitch;
		}
		
		float finalSpin = spin;
		float finalPitch = pitch;

		if (spinLock != RotationLock.Locked)
		{
			finalSpin += target.eulerAngles.y;
		}
		if (pitchLock != RotationLock.Locked)
		{
			finalPitch += target.eulerAngles.x;
		} 
		
		Quaternion rotation = Quaternion.Euler (finalPitch, finalSpin, roll);
		targetRotation = rotation;
		
		centrePosition = target.position + (Vector3.up * verticalOffset) + (rotation * Vector3.right * horizontalOffset);
		targetPosition = centrePosition - (rotation * Vector3.forward * distance);
	}

	public void beganCameraTransition()
	{
		targetRotation = transform.rotation;
		targetPosition = transform.position;
	}
}
