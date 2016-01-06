using UnityEngine;
using System.Collections;

public class DEMO_UOC_GUI : MonoBehaviour {

	public UltimateOrbitCamera target;

	private int height = 0;

	private bool showOrbitOptions = false;
	private bool showMouseOptions = false;
	private bool showKeyboardOptions = false;
	private bool showAutoRotateOptions = false;
	private bool showSpinOptions = false;
	private bool showCollisionOptions = false;

	private int LabelWidth = 80;
	private int SliderWidth =  180;
	private Vector2 scrollPos = new Vector2(0f,0f);

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target.invertAxisX)
		{
			target.invertXValue = -1;
		}
		else
		{
			target.invertXValue = 1;
		}
		if(target.invertAxisY)
		{
			target.invertYValue = -1;
		}
		else
		{
			target.invertYValue = 1;
		}
		if(target.invertAxisZoom)
		{
			target.invertZoomValue = -1;
		}
		else
		{
			target.invertZoomValue = 1;
		}
		if(target.autoRotateReverse)
		{
			target.autoRotateReverseValue = -1;
		}
		else
		{
			target.autoRotateReverseValue = 1;
		}
	}

	void OnGUI()
	{
		height = 204;
		if(showOrbitOptions)
		{
			height += 570;
		}
		if(showMouseOptions)
		{
			height += 130;
		}
		if(showKeyboardOptions)
		{
			height += 40;
		}
		if(showAutoRotateOptions)
		{
			height += 80;
		}
		if(showSpinOptions)
		{
			height += 100;
		}
		if(showCollisionOptions)
		{
			height += 80;
		}

		if(GUI.Button(new Rect(Screen.width-105,5,100,50),"Reset"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}


		GUI.Box(new Rect(10,10,340,Mathf.Min(height,Screen.height-20)),"");
		GUILayout.BeginArea(new Rect(12,12,336,Mathf.Min(height-4,Screen.height-24)));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		showOrbitOptions = GUILayout.Toggle(showOrbitOptions," Show Orbit Options");
		GUILayout.Space(4);
		if(showOrbitOptions)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Axis X Speed",GUILayout.Width(LabelWidth));
			target.xSpeed = GUILayout.HorizontalSlider(target.xSpeed,0f,2f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.xSpeed.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Axis Y Speed",GUILayout.Width(LabelWidth));
			target.ySpeed = GUILayout.HorizontalSlider(target.ySpeed,0f,2f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.ySpeed.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Zoom Sensitivity",GUILayout.Width(LabelWidth));
			target.zoomSpeed = GUILayout.HorizontalSlider(target.zoomSpeed,0f,50f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.zoomSpeed.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Axis X Dampening",GUILayout.Width(LabelWidth));
			target.dampeningX = GUILayout.HorizontalSlider(target.dampeningX,0.01f,0.99f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.dampeningX.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Axis Y Dampening",GUILayout.Width(LabelWidth));
			target.dampeningY = GUILayout.HorizontalSlider(target.dampeningY,0.01f,0.99f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.dampeningY.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Zoom Smoothing",GUILayout.Width(LabelWidth));
			target.smoothingZoom = GUILayout.HorizontalSlider(target.smoothingZoom,0.01f,1f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.smoothingZoom.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Min Distance",GUILayout.Width(LabelWidth));
			target.minDistance = Mathf.Min (target.maxDistance,GUILayout.HorizontalSlider(target.minDistance,1f,50f,GUILayout.Width(SliderWidth)));
			GUILayout.Label(target.minDistance.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("MaxDistance",GUILayout.Width(LabelWidth));
			target.maxDistance = Mathf.Max (target.minDistance,GUILayout.HorizontalSlider(target.maxDistance,1f,50f,GUILayout.Width(SliderWidth)));
			GUILayout.Label(target.maxDistance.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Limit Angle X",GUILayout.Width(LabelWidth));
			target.limitX = GUILayout.Toggle(target.limitX,"");
			GUILayout.EndHorizontal();
			if(!target.limitX)
				GUI.color = new Color(0.5f,0.5f,0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Min Angle X",GUILayout.Width(LabelWidth));
			target.xMinLimit = Mathf.Min (target.xMaxLimit,GUILayout.HorizontalSlider(target.xMinLimit,-180f,180f,GUILayout.Width(SliderWidth)));
			GUILayout.Label(target.xMinLimit.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Angle X",GUILayout.Width(LabelWidth));
			target.xMaxLimit = Mathf.Max (target.xMinLimit,GUILayout.HorizontalSlider(target.xMaxLimit,-180f,180f,GUILayout.Width(SliderWidth)));
			GUILayout.Label(target.xMaxLimit.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("X Limit Offset",GUILayout.Width(LabelWidth));
			target.xLimitOffset = GUILayout.HorizontalSlider(target.xLimitOffset,0f,360f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.xLimitOffset.ToString("F"));
			GUILayout.EndHorizontal();
			GUI.color = new Color(1f,1f,1f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Limit Angle Y",GUILayout.Width(LabelWidth));
			target.limitY = GUILayout.Toggle(target.limitY,"");
			GUILayout.EndHorizontal();
			if(!target.limitY)
				GUI.color = new Color(0.5f,0.5f,0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Min Angle Y",GUILayout.Width(LabelWidth));
			target.yMinLimit = Mathf.Min (target.yMaxLimit,GUILayout.HorizontalSlider(target.yMinLimit,-180f,180f,GUILayout.Width(SliderWidth)));
			GUILayout.Label(target.yMinLimit.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Angle Y",GUILayout.Width(LabelWidth));
			target.yMaxLimit = Mathf.Max (target.yMinLimit,GUILayout.HorizontalSlider(target.yMaxLimit,-180f,180f,GUILayout.Width(SliderWidth)));
			GUILayout.Label(target.yMaxLimit.ToString("F"));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Y Limit Offset",GUILayout.Width(LabelWidth));
			target.yLimitOffset = GUILayout.HorizontalSlider(target.yLimitOffset,0f,360f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.yLimitOffset.ToString("F"));
			GUILayout.EndHorizontal();
			GUI.color = new Color(1f,1f,1f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Invert Axis X",GUILayout.Width(LabelWidth));
			target.invertAxisX = GUILayout.Toggle(target.invertAxisX,"");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Invert Axis Y",GUILayout.Width(LabelWidth));
			target.invertAxisY = GUILayout.Toggle(target.invertAxisY,"");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Invert Zoom",GUILayout.Width(LabelWidth));
			target.invertAxisZoom = GUILayout.Toggle(target.invertAxisZoom,"");
			GUILayout.EndHorizontal();
		}
		GUILayout.Space(8);
		showMouseOptions = GUILayout.Toggle(showMouseOptions," Show Mouse Input Options");
		GUILayout.Space(4);
		if(showMouseOptions)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Mouse Controls",GUILayout.Width(LabelWidth));
			target.mouseControl = GUILayout.Toggle(target.mouseControl,"");
			GUILayout.EndHorizontal();
			if(!target.mouseControl)
				GUI.color = new Color(0.5f,0.5f,0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Click To Rotate",GUILayout.Width(LabelWidth));
			target.clickToRotate = GUILayout.Toggle(target.clickToRotate,"");
			GUILayout.EndHorizontal();
			if(!target.clickToRotate)
				GUI.color = new Color(0.5f,0.5f,0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Left Click",GUILayout.Width(LabelWidth));
			target.leftClickToRotate = GUILayout.Toggle(target.leftClickToRotate,"");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Right Click",GUILayout.Width(LabelWidth));
			target.rightClickToRotate = GUILayout.Toggle(target.rightClickToRotate,"");
			GUILayout.EndHorizontal();
			GUI.color = new Color(1f,1f,1f);
		}
		GUILayout.Space(8);
		showKeyboardOptions = GUILayout.Toggle(showKeyboardOptions," Show Keyboard Input Options");
		GUILayout.Space(4);
		if(showKeyboardOptions)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Keyboard Controls",GUILayout.Width(LabelWidth));
			target.keyboardControl = GUILayout.Toggle(target.keyboardControl,"");
			GUILayout.EndHorizontal();
		}	
		GUILayout.Space(8);
		showAutoRotateOptions = GUILayout.Toggle(showAutoRotateOptions," Show Auto-Rotate Options");
		GUILayout.Space(4);
		if(showAutoRotateOptions)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Auto Rotate",GUILayout.Width(LabelWidth));
			target.autoRotateOn = GUILayout.Toggle(target.autoRotateOn,"");
			GUILayout.EndHorizontal();
			if(!target.autoRotateOn)
				GUI.color = new Color(0.5f,0.5f,0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Reverse",GUILayout.Width(LabelWidth));
			target.autoRotateReverse = GUILayout.Toggle(target.autoRotateReverse,"");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Rotate Speed",GUILayout.Width(LabelWidth));
			target.autoRotateSpeed = GUILayout.HorizontalSlider(target.autoRotateSpeed,0.01f,20f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.autoRotateSpeed.ToString("F"));
			GUILayout.EndHorizontal();
			GUI.color = new Color(1f,1f,1f);
		}
		GUILayout.Space(8);
		showSpinOptions = GUILayout.Toggle(showSpinOptions," Show Spin Options");
		GUILayout.Space(4);
		if(showSpinOptions)
		{
			GUILayout.Label("Hold Left-CTRL and throw to spin.");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Spin Enabled",GUILayout.Width(LabelWidth));
			target.SpinEnabled = GUILayout.Toggle(target.SpinEnabled,"");
			GUILayout.EndHorizontal();
			if(!target.SpinEnabled)
				GUI.color = new Color(0.5f,0.5f,0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Spin Speed",GUILayout.Width(LabelWidth));
			target.maxSpinSpeed = GUILayout.HorizontalSlider(target.maxSpinSpeed,0.01f,5f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.maxSpinSpeed.ToString("F"));
			GUILayout.EndHorizontal();
			GUI.color = new Color(1f,1f,1f);

		}
		GUILayout.Space(8);
		showCollisionOptions = GUILayout.Toggle(showCollisionOptions," Show Collision Options");
		GUILayout.Space(4);
		if(showCollisionOptions)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Collision Enabled",GUILayout.Width(LabelWidth));
			target.cameraCollision = GUILayout.Toggle(target.cameraCollision,"");
			GUILayout.EndHorizontal();
			if(!target.cameraCollision)
				GUI.color = new Color(0.5f,0.5f,0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Collision Radius",GUILayout.Width(LabelWidth));
			target.collisionRadius = GUILayout.HorizontalSlider(target.collisionRadius,0.01f,1f,GUILayout.Width(SliderWidth));
			GUILayout.Label(target.collisionRadius.ToString("F"));
			GUILayout.EndHorizontal();
			GUI.color = new Color(1f,1f,1f);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
