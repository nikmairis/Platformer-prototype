using UnityEngine;
using System.Collections;

namespace testprojekts{
public class GrappleInputManager : MonoBehaviour {
	
	GrappleScript grapple;
	public Camera cam;

	public float angleStep = 1;
	/* The angle around the target that the rope can attach to
	 * i.e 90 means 90 degrees clockwise + 90 counter clockwise.*/
	[Range(0.0f,360.0f)]
	public float angleTolerance = 90;
	
	void Start()
	{
		grapple = GetComponent<GrappleScript>();
		cam = Camera.main;
	}
	
	void Update()
	{
		UpdateInput();
	}
	
	private void UpdateInput () {
		if(Input.GetKeyDown(KeyCode.F))
		{
			//this.GetComponent<Player>().newGrapple = true;
			//this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			//this.GetComponent<Rigidbody2D>().freezeRotation = true;
			// Find mouse position
			Vector3 mouseInput = new Vector3(Input.mousePosition.x,Input.mousePosition.y,10);
			Vector2 mouseClick = cam.ScreenToWorldPoint(mouseInput);
			
			// Find ray direction and raycast
			Vector2 rayDirection = mouseClick - (Vector2)this.transform.position;
			RaycastHit2D hit = Physics2D.Raycast((Vector2)this.transform.position , rayDirection , grapple.grapplingHookRange, ~(1<<grapple.playerLayer));
			float angle = angleStep;
			Quaternion rot;

			// If the raycast does not hit anything, loop raycast until object is hit
			while(hit.collider == null && angle<angleTolerance)
			{
				rot = Quaternion.AngleAxis(angle , Vector3.forward);
				hit = Physics2D.Raycast((Vector2)this.transform.position , rot*rayDirection, grapple.grapplingHookRange, ~(1<<grapple.playerLayer));
				
				if(hit.collider!=null)
					break;
				
				rot = Quaternion.AngleAxis(-angle , Vector3.forward);
				hit = Physics2D.Raycast((Vector2)this.transform.position , rot*rayDirection, grapple.grapplingHookRange, ~(1<<grapple.playerLayer));
				angle+=angleStep;
				
			}
			// if something is hit, and that is not the player
			if(hit.collider != null && hit.collider.gameObject.layer != grapple.playerLayer )
			{
				grapple.AttachRope(hit.point);
			}
		}
		// Check for rope release
		else if(Input.GetKeyUp(KeyCode.F))
		{
			//this.GetComponent<Player>().newGrapple = false;
			//this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
			grapple.ReleaseRope();
		}

		// Setting reeling and paying out
		grapple.reeling_in = Input.GetKey(KeyCode.F);
		grapple.paying_out = Input.GetKey(KeyCode.R);

	}
}
}