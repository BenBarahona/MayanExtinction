using UnityEngine;
using System.Collections;

public class CameraOrbitScript : MonoBehaviour {

	public CameraProperties properties;

	public string mouseAxisX = "Mouse X";
	public string mouseAxisY = "Mouse Y";
	public bool invertAxisX = false;
	public bool invertAxisY = false;
	public float xSpeed = 1f;
	public float ySpeed = 1f;
	public float dampeningX = 0.9f;
	public float dampeningY = 0.9f;

	public bool cameraCollision = false;
	public float collisionRadius = 0.25f;
	
	private float xVelocity = 0f;
	private float yVelocity = 0f;	
	private float targetDistance = 10f;
	private float x = 0f;
	private float y = 0f;
	private Vector3 position;
	
	[HideInInspector]
	public int invertXValue = 1;
	[HideInInspector]
	public int invertYValue = 1;
	[HideInInspector]
	public Camera _camera;
	
	private Ray ray;
	private RaycastHit hit;
	
	private Transform _transform;
	
	void Awake ()
	{
		Camera cam = GetComponent<Camera>();
		if (cam)
		{
			_camera = cam;
		}
	}
	
	void Start () 
	{
		targetDistance = properties.distance;
		if (invertAxisX)
		{
			invertXValue = -1;
		}
		else
		{
			invertXValue = 1;
		}
		if (invertAxisY)
		{
			invertYValue = -1;
		}
		else
		{
			invertYValue = 1;
		}
		
		_transform = transform;
		
		if (GetComponent<Rigidbody>() != null)
			GetComponent<Rigidbody>().freezeRotation = true;
		
		x = properties.initialAngleX;
		y = properties.initialAngleY;
		_transform.Rotate(new Vector3(0f,properties.initialAngleX,0f),Space.World);
		_transform.Rotate(new Vector3(properties.initialAngleY,0f,0f),Space.Self);
		
		position = _transform.rotation * new Vector3(0.0f, 0.0f, -properties.distance) + properties.transform.position;
	}
	
	void Update () 
	{
		if(properties != null)
		{
			#region Input

			#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR
			if(Input.touches.Length == 1)
			{
				xVelocity += Input.GetTouch(0).deltaPosition.x * xSpeed * invertXValue * 0.2f;
				yVelocity -= Input.GetTouch(0).deltaPosition.y * ySpeed * invertYValue * 0.2f;
			}
			#else
				if(Input.GetMouseButton(0))
				{
					xVelocity += Input.GetAxis(mouseAxisX) * xSpeed * invertXValue;
					yVelocity -= Input.GetAxis(mouseAxisY) * ySpeed * invertYValue;
				}

			#endif
			#endregion
			
			#region Apply_Rotation_And_Position
			if(properties.limitX)
			{
				if(x + xVelocity < properties.xMinLimit + properties.xLimitOffset)
				{
					xVelocity = (properties.xMinLimit + properties.xLimitOffset) - x;
				}
				else if(x + xVelocity > properties.xMaxLimit + properties.xLimitOffset)
				{
					xVelocity = (properties.xMaxLimit + properties.xLimitOffset) - x;
				}
				x += xVelocity;
				_transform.Rotate(new Vector3(0f,xVelocity,0f),Space.World);
			}
			else
			{
				_transform.Rotate(new Vector3(0f,xVelocity,0f),Space.World);
			}
			if(properties.limitY)
			{
				if(y + yVelocity < properties.yMinLimit + properties.yLimitOffset)
				{
					yVelocity = (properties.yMinLimit + properties.yLimitOffset) - y;
				}
				else if(y + yVelocity > properties.yMaxLimit + properties.yLimitOffset)
				{
					yVelocity = (properties.yMaxLimit + properties.yLimitOffset) - y;
				}
				y += yVelocity;
				_transform.Rotate(new Vector3(yVelocity,0f,0f),Space.Self);
			}
			else
			{
				_transform.Rotate(new Vector3(yVelocity,0f,0f),Space.Self);
			}

			if(cameraCollision)
			{
				ray = new Ray(properties.transform.position,(_transform.position - properties.transform.position).normalized);
				if (Physics.SphereCast(ray.origin, collisionRadius, ray.direction, out hit, properties.distance))
				{
					properties.distance = hit.distance;
				}
			}
			#endregion
			
			position = _transform.rotation * new Vector3(0.0f, 0.0f, -properties.distance) + properties.transform.position;
			_transform.position = position;

			xVelocity *= dampeningX;
			yVelocity *= dampeningY;
		}
		else
		{
			Debug.LogWarning("Orbit Cam - No Target Given");
		}
	}
}
