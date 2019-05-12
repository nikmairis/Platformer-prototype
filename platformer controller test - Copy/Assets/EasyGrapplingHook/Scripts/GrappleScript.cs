/* Copyright (C) Tool Games - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Keelan Moore <km.keelan@gmail.com>, June 2015
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

[RequireComponent (typeof(LineRenderer))]
[RequireComponent (typeof(Rigidbody2D))]

public class GrappleScript : MonoBehaviour {
	
	private bool pivotAttached = false;					// Are you currently swinging
	
	private float ropeLength;							// Total length of rope


	public Vector2 pivotPoint;							// Current active pivot
	private List<Vector3> pivotList;					// List of all rope bends
	private List<bool> currentSwingDirection;			// Keeps Track of the direction of each bend


	public float reelInSpeed = 1;						// Speed in which you shorten the rope
	public float payOutSpeed = 1;						// Speed in which you lengthen the rope
	public bool reeling_in = false;						// Are you currently shortening the rope
	public bool paying_out = false;						// Are you currently extending the rope
	
	private LineRenderer rope;							// Rope renderer	
	private Rigidbody2D rigid_body;						// Player rigidbody

	
	private Vector2 vectorToPivot;						// Vector towards grapple/ropebend
	private float distanceToPivot;						// Distance to grapple/rope bend
	private Vector2 directionToPivot;					// Unit vector from player to grapple/ropebend

	public float ropeBendTolerance = 0.01f;				// Minimum distance rope bends can be added
	public float grapplingHookRange = 100f;				// Max range the grappling can be fired
	public int playerLayer = 8;							// Layer the player gameobject is on
	public bool autoSetLayer = true;
	public Vector2 ropeBasePoint = new Vector2(0,0);	// Point on the player the rope is attached - local space
	public bool allowRotation = true;				// Set orientation of player towards rope
	public bool ropeCollisions = true;					// Can the rope collide with objects
	public float strength = 1;
	void Start()
	{
		// Initialise rope renderer
		rope = GetComponent<LineRenderer>();
		rope.SetPosition(0,transform.position);
		rope.SetPosition(1,transform.position);

		// Initialise lists
		pivotList = new List<Vector3>();
		currentSwingDirection = new List<bool>();
		
		// Store rigidbody reference
		rigid_body = this.GetComponent<Rigidbody2D>();

		if(autoSetLayer)
			playerLayer = this.gameObject.layer;
	}

	void FixedUpdate () {


		
		if(pivotAttached)	// If currently swinging
		{
			// If start swing, add grapple point
			if(pivotList.Count==0)
			{
				pivotList.Add(pivotPoint);
				SetRopeLength();
			}

			if(ropeCollisions) // If rope collisions with world are enabled
			{
				//vector from player center to pivot point
				vectorToPivot = (Vector2)(pivotList[pivotList.Count-1] - this.transform.position);	
				//Distance to pivot
				distanceToPivot = vectorToPivot.magnitude;
				//unit vector in directon of pivot
				directionToPivot = vectorToPivot.normalized;
				
				// Raycast from player to pivot point - checking line of sight
				RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position,
				                                     directionToPivot,distanceToPivot , ~(1<<playerLayer));
				

				if(hit.collider!=null)	// if raycast hits
				{

					if(	Vector2.Distance(hit.point,pivotList[pivotList.Count-1]) > ropeBendTolerance ) 
					// If hit point meets bend requirements
					{
						// Then add bend
						AddRopeBend(hit);
						// Add bend to list
						pivotList.Add(pivotPoint);
						// Reset rope length
						SetRopeLength();
						// Detect direction of swing
						currentSwingDirection.Add(isBendClockwise());

					}
				}
				
				// if the rope has bends in it
				if(currentSwingDirection.Count>0)
				{
					// if the rope bend has changed direction, then remove bend
					if(currentSwingDirection[currentSwingDirection.Count-1]!= isBendClockwise())
					{
						// Remove inactive pivot and bend direction
						pivotList.RemoveAt(pivotList.Count-1);
						currentSwingDirection.RemoveAt(currentSwingDirection.Count-1);
						// Reset rope length
						SetRopeLength();
					}
				}
			}

			//vector from player center to pivot point
			vectorToPivot = (Vector2)(pivotList[pivotList.Count-1] - this.transform.position);	
			//Distance to pivot
			distanceToPivot = vectorToPivot.magnitude;
			//unit vector in directon of pivot
			directionToPivot = vectorToPivot.normalized;
			// Speed in the direction of the rope
			float speedTowardsPivot = Vector2.Dot(rigid_body.velocity, directionToPivot);

			NeutraliseGravity();

			// If moving away from pivot and distance to pivot is greater than rope length
			if(speedTowardsPivot<=0 &&
			   distanceToPivot >= ropeLength)
			{
				// Cancel out gravity in direction of rope
				rigid_body.AddForce(directionToPivot* Vector2.Dot(Physics2D.gravity , directionToPivot)*-1);
				// Cancelling speed in direction of rope 
				rigid_body.velocity = GetComponent<Rigidbody2D>().velocity - (Vector2)(speedTowardsPivot*directionToPivot);
			}
			if(reeling_in)	// If currently reeling in
			{
				if(speedTowardsPivot <= reelInSpeed) { // If speed towards grapple point is less than reeling speed
					// Add reeling speed
					rigid_body.velocity = GetComponent<Rigidbody2D>().velocity + (Vector2)(reelInSpeed*directionToPivot);
					
				}
				// Shorten rope length by reeling speed
				ropeLength = ropeLength - reelInSpeed*Time.deltaTime*1.3f;
			}
			else if(paying_out)	// If paying out rope
			{
				if(speedTowardsPivot <= payOutSpeed) {	// If speed is less than payout speed
					// Add paying out velocity
					rigid_body.velocity = GetComponent<Rigidbody2D>().velocity - (Vector2)(payOutSpeed*directionToPivot);
					
				}
				// Lengthen rope by paying out speed
				ropeLength = ropeLength + payOutSpeed*Time.deltaTime;

			}
			// If player orientation towards rope is active
			if(!allowRotation)
			transform.rotation = Quaternion.FromToRotation(Vector2.up,directionToPivot);
		}
	}
	
	void Update()
	{
		renderRope();	
	}
	
	public void SetRopeLength()
	{
		ropeLength = Vector2.Distance(pivotList[pivotList.Count-1],this.transform.position);
	}
	void renderRope()
	{
		if(pivotAttached)
		{
			// Set initial rope points
			rope.SetVertexCount(1+pivotList.Count);
			rope.SetPosition(0,transform.TransformPoint(ropeBasePoint));
			// Loop over rope bends, add positions
			for(int i = 0 ; i<pivotList.Count; i++)
			{
				rope.SetPosition(i+1,pivotList[pivotList.Count-1-i]);
			}
		}
		else
		{
			// If not attached to rope, clear lists
			pivotList.Clear();
			currentSwingDirection.Clear();
			rope.SetVertexCount(1);
			rope.SetPosition(0,transform.position);
		}
	}
	
	void AddRopeBend(RaycastHit2D hit)
	{
		// Calculate direction from object center to hit point
		Vector3 direc = (Vector3)hit.point - hit.transform.position;
		// Move pivot point margenally outside object
		pivotPoint =  hit.transform.position + direc.normalized*(direc.magnitude + 0.01f);
	}
	
	bool isBendClockwise()
	{
		// Store player positition
		Vector2 playerPos = transform.position;
		// Calculate direction of bend
		Vector2 vectorPlayerToCurrentPivot = (Vector2)pivotList[pivotList.Count-1] - playerPos;
		Vector2 vectorCurrentPivotToLastPivot = pivotList[pivotList.Count-2] - pivotList[pivotList.Count-1];
		float dot = (vectorPlayerToCurrentPivot.y*vectorCurrentPivotToLastPivot.x - vectorPlayerToCurrentPivot.x*vectorCurrentPivotToLastPivot.y);
		return dot<0;
	}
	
	public void AddPivot(Vector3 newPivot)
	{
		pivotList.Add(newPivot);
	}
	public void ReleaseRope()
	{
		pivotAttached = false;

		if(allowRotation)
			return;
		rigid_body.constraints = RigidbodyConstraints2D.FreezeRotation;
		rigid_body.constraints = RigidbodyConstraints2D.None;
	}
	public void AttachRope(Vector2 grapplePoint)
	{
		pivotPoint = grapplePoint;
		pivotAttached = true;
	}
	public void SetReelingIn(bool currentReelingState)
	{
		reeling_in = currentReelingState;
	}
	public void SetPayingOut(bool currentPayingState)
	{
		paying_out = currentPayingState;
	}

	void NeutraliseGravity()
	{
		rigid_body.AddForce(directionToPivot * Vector2.Dot(Physics2D.gravity * rigid_body.gravityScale * strength , directionToPivot) *-1);
	}

}
