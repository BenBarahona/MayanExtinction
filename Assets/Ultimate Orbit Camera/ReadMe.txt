====== ULTIMATE ORBIT CAMERA ======
=====
===== Contents:
=====
===== 1: Setup
===== 2: Feature Descriptions
===== 3: Tweaking Options
===== 4: Notes
=====

1. ==== SETUP ====

- 1. Attach the UltimateOrbitCamera Component to your chosen camera.
- 2. Drag and drop your target Transform into the "Target" Attribute. (This is the object in your scene you want the camera to orbit.)
- 3. Enable/Disable features and tweak for desired effect.


2. ==== FEATURE DESCRIPTIONS ====

Orbit Camera - The camera will orbit the target object at a constant distance. This distance can be changed by zooming in or out. The movement can be smoothed to give a nicer effect, and the rotation of the camera can be limited to set angles.

Mouse/Keyboard Input - The mouse or keyboard can be used to control the camera (or both). The mouse controls move the camera by using the mouse input axes, and zoom is controlled by the scroll wheel axis. The keyboard uses the "Horizontal" and "Vertical" axes for movement and either use customizeable keys or a seperate input axis for zoom. All the specified input axes can be changed.

Auto Rotate - When turned on this will cause the camera to turn at a constant speed.

Throw Spin - When enabled, holding down the spin key and "throwing" the camera will cause the camera to turn constantly at the thrown speed. The default key is 'Left Control'.

Collision - When collision is enabled, the camera will collider with other colliders in the scene. When this happens, the camera will zoom in so it still has a view of the target object. Setting an object's layer to "Ignore Raycast" will make the camera ignore and pass through it.

Touch Controls - Touch controls will be automatically used if the device (or editor platform) is Android, iOS or Windows Phone 8. This will make the mouse/keyboard settings redundant, but will still use the orbit camera settings.


3. ==== TWEAKING OPTIONS ====


=== ORBIT CAMERA OPTIONS === 

- Initial Angle X/Y - Sets initial rotation on start.

- X/Y Speed - Sets respective axes sensitivities.

- zoom Speed - Sets zoom sensitivity.

- Dampening X/Y - Sets orbit smoothing amount between 0 and 1. This is deceleration on the orbit. 0 is instant deceleration to stop.  The   closer to 1 it is, the slower the camera will decellerate.

- Zoom Smoothing - Sets zoom smooth speed between 0 and 1. This is a lerp function. 1 is no smoothing, Closer to 0 slows the camera down.

- Start Distance - Set initial distance to target.

- Min Distance - Set Minimum zoom distance to target.

- Max Distance - Set Maximum distance to target.

- Limit X - Enables/Disables limiting horizontal rotation. If unchecked, the camera can swing around 360 Degrees.

- X Min - Minimum horizontal angle.

- X Max - Maximum horizontal angle.

- X Limit Offset - Offsets the horizontal limiting arc by specified number of degrees.

- Limit Y - Enables/Disables limiting Pitch. If unchecked, the camera can swing around 360 Degrees.

- Y Min - Minimum pitch angle.

- Y Max - Maximum pitch angle.

- Y Limit Offset - Offsets the Vertical limiting arc by specified number of degrees.

- Invert Axis X - Reverses horizontal camera controls (Left is right, right is left).

- Invert Axis Y - Reverses pitch camera controls (up is down, down is up).

- Invert Zoom - Reverses zoom controls.




=== MOUSE INPUT OPTIONS ===

- Mouse Control - Enables/Disables Mouse Control.

- Click To Rotate - If checked, a mouse button will need to be held down to move the camera. Otherwise the camera will move whenever the   mouse is moved.

- Left Click - If checked, and Click to Rotate is enabled, the camera when move when left click is held down.

- Right Click - If checked, and Click to Rotate is enabled, the camera when move when right click is held down.

- Axis X Name - Horizontal mouse input axis, only needed if you change the name of the axis in Input settings.

- Axis Y Name - Vertical mouse input axis, only needed if you change the name of the axis in Input settings.

- Zoom Axis Name - Scroll wheel mouse input axis, only needed if you change the name of the axis in Input settings.




=== KEYBOARD INPUT OPTIONS === 

- Keyboard Control - Enables/Disables keyboard controls

- Axis X Name - Horizontal rotation input axis. Default is "Horizontal", by default this is the left and right arrow keys and A & D keys.

- Axis Y Name - Pitch rotation input axis. Default is "Vertical", by default this is the up and down arrow keys and W & S keys.

- Use Zoom Input Axis - Enable to set a button input axis instead of setting keys. If disabled the specified zoom keys will be used.

- Zoom In Key - Set key to zoom in.

- Zoom Out Key - Set key to zoom out.

- Zoom Axis Name - If use zoom input axis is enabled, this is the name of the Input axis to use.




=== AUTO ROTATE OPTIONS === 

- Auto Rotate - Enables/Disables auto-rotation.

- Reverse - Reverses the direction of the auto-rotation.

- Speed - Sets the speed of the auto-rotation.




=== SPIN OPTIONS === 

- Spin Enabled - Enables/Disables throw spinning.

- Max Speed - Sets the max spin speed.

- Use Button Input Axis - Enable to set a button input axis instead of setting a key. If disabled the specified key will be used.

- Spin Key - Sets the spin key.

- Button Axis name - If use button input axis is enabled, this is the name of the Input axis to use.




=== COLLISION OPTIONS === 

- Collision Enabled - Enables/Disables collision detection.

- Collision Radius - Sets the collision radius of the camera.





4. ==== NOTES ====

* 'Throw spin' will not work with touch screen (as there is no key to hold down).

* Input axes can be modified within unity under Edit->Project Settings->Input

* When using auto rotate, the camera controls will still work. If this is not intended, make sure to turn them off or set the control speed to 0.

* Collision can cause the camera to zoom closer than the minimum distance.

* When collision is enabled, if the collision radius is bigger than the target object, the camera will collide with the target object. To fix this, add the target object to the "Ignore Raycast" layer.

*Inverted axes are setup on start so if you want to invert the axes at runtime via script, you must invert the corresponding 'invertValue' integer attribute. (These are: 'invertXValue', 'invertYValue', 'invertZoomValue' and 'autoRotateReverseValue'. Just multiply by -1)











