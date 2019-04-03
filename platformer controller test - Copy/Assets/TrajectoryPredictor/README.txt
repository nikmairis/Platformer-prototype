Thanks for using my trajectory prediction script!


Setup:
There are a couple of options for using the script
You can add it via a component and reference it in script to get trajectory, debug, and hit info
To do this:
	1. Drag the Trajectory Predictor component onto your mesh gameobject.
	  (you can find it under Physics->Trajectory Predictor)

	2. Here is where you can adjust the settings of the component in the inspector to your needs.

	3. By default, it wont do anything. You can change the debug options to make it draw a trajectory line
	   in various situations, this only works if the object has a rigidbody component.

	4. In order to call functions and get data from the component you need to assign its instance to a
	   variable. This can be as simple as:
	   TrajectoryPredictor tp = GetComponent<TrajectoryPredictor>();

	5. Now you can call one of the predict functions to perform a prediction, to do this
	   you need to call either Predict2D() or Predict3D() from the instance mentioned earlier
	   Call it and input the necessary values (you can input a rigidbody or your own velocity and position)
	   and it will perform the prediction
	   
	6. To get the data from the prediction, you can reference the hitInfo3D, hitInfo2D, and predictionPoints
	   variables of the Trajectory Predictor instance where:
	   hitInfo3D and hitInfo2D are RaycastHit and RaycastHit2D respectively, these contain information about
	   where the trajectory collides with something (if you chose to use Stop On Collision)
	   predictionPoints is a list of Vector3's that describes all the points in the prediction line


Or, you can call on the Trajectory Predictor class and use some of its static functions
To use these simply Call them and input the propper values and it will return the requested information of which
can be assigned to a variable. Example:
	Vector3[] points = TrajectoryPredictor.GetPoints3D(transform.position, transform.forward * 20f, Physics.gravity);

The downside of using the static functions is that multiple predictions would have to be made to obtain both the
prediction points and the hit info, as well as being slightly slower and less efficient to be done every frame
although not by much.

The list of the available static functions is as follows

	public static Vector3[] GetPoints3D(Vector3 startPos, Vector3 velocity, Vector3 gravity, float linearDrag = 0f,
		float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}

	public static Vector3[] GetPoints2D(Vector3 startPos, Vector2 velocity, Vector2 gravity, float linearDrag = 0f,
		float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}

	public static Vector3[] GetPoints3D(Rigidbody rb, float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}

	public static Vector3[] GetPoints2D(Rigidbody2D rb, float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}

	public static RaycastHit GetHitInfo3D(Vector3 startPos, Vector3 velocity, Vector3 gravity, float linearDrag = 0f,
		float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}

	public static RaycastHit2D GetHitInfo2D(Vector3 startPos, Vector2 velocity, Vector2 gravity, float linearDrag = 0f,
		float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}

	public static RaycastHit GetHitInfo3D(Rigidbody rb, float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}

	public static RaycastHit2D GetHitInfo2D(Rigidbody2D rb, float accuracy = 0.985f, int iterationLimit = 150, bool stopOnCollision = true){...}



And thanks to you, hopefully all the settings have good tooltips and the script does everything you need.
If not, feel free to send me a message :]