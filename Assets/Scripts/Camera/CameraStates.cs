using UnityEngine;
using System.Collections;

public enum CameraStates {
	ORBIT,
	LOCK_CAMERA
};

public enum RotationLock {Free, Locked, Limited };
public enum CameraType {Target, Fixed, Normal}

public enum MouseState { Normal, SingleClick, RightClick, DoubleClick, HeldDown, LetGo };
public enum DragState { None, Inventory, _Camera };
public enum GameState {Paused, Resumed, Inventory}


