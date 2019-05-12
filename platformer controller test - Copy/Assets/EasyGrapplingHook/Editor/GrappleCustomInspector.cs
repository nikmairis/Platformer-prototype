/* Copyright (C) Tool Games - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Keelan Moore <km.keelan@gmail.com>, June 2015
 */

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GrappleScript))]

public class GrappleCustomInspector : Editor {
	
	
	SerializedProperty reelInSpeed;						// Speed in which you shorten the rope
	GUIContent label_reelInSpeed;
	SerializedProperty payOutSpeed;						// Speed in which you lengthen the rope
	GUIContent label_payOutSpeed;
	
	
	SerializedProperty grapplingHookRange;				// Max range the grappling can be fired
	GUIContent label_grapplingHookRange;
	SerializedProperty autoSetLayer;
	GUIContent label_autoSetLayer;
	SerializedProperty playerLayer;							// Layer the player gameobject is on
	GUIContent label_playerLayer;
	SerializedProperty ropeBasePoint;					// Point on the player the rope is attached - local space
	GUIContent label_ropeBasePoint;

	SerializedProperty strength;
	GUIContent label_strength;
	SerializedProperty ropeBendTolerance;				// Minimum distance rope bends can be added
	GUIContent label_ropeBendTolerance;
	SerializedProperty allowRotation;				// Set orientation of player towards rope
	GUIContent label_allowRotation;
	SerializedProperty ropeCollisions;					// Can the rope collide with objects
	GUIContent label_ropeCollisions;
	
	
	void OnEnable()
	{
		reelInSpeed = serializedObject.FindProperty("reelInSpeed");
		label_reelInSpeed = new GUIContent ("Reel In Speed", "The speed at which the rope retracts");
		
		payOutSpeed = serializedObject.FindProperty("payOutSpeed");
		label_payOutSpeed = new GUIContent("Pay Out Speed","The speed at which the rope extends");
		
		ropeBendTolerance = serializedObject.FindProperty("ropeBendTolerance");
		label_ropeBendTolerance = new GUIContent("Rope Bend Tolerance","The minimum distance between rope bend points");
		
		grapplingHookRange = serializedObject.FindProperty("grapplingHookRange");
		label_grapplingHookRange = new GUIContent("Grappling Hook Range","The maximum distance the grappling hook can fire\nUsed in the input script");
		
		autoSetLayer = serializedObject.FindProperty("autoSetLayer");
		label_autoSetLayer = new GUIContent("Auto Set Layer","Autodetects the players layer");
		
		playerLayer = serializedObject.FindProperty("playerLayer");
		label_playerLayer = new GUIContent("Player Layer","The players layer\nThis is so the rope doesn't attach to the player");
		
		ropeBasePoint = serializedObject.FindProperty("ropeBasePoint");
		label_ropeBasePoint = new GUIContent("Rope Base Point","The point in local space to the player that\nthe rope attaches to");
		
		allowRotation = serializedObject.FindProperty("allowRotation");
		label_allowRotation = new GUIContent("Allow Rotations","Allow rotational physics interactions\nwhilst player is attached to rope");
		
		ropeCollisions = serializedObject.FindProperty("ropeCollisions");
		label_ropeCollisions = new GUIContent("Allow Rope Collisions","Allows the rope to interact and wrap around static objects");
	
		strength = serializedObject.FindProperty("strength");
		label_strength = new GUIContent("Rope Strength" , "Set 1 for default rigidbody settings\nIncrease if the rope doesn't stop the\nplayer falling");
	}
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Rope Speed Variables" , EditorStyles.boldLabel);
		EditorGUILayout.Space();
		EditorGUI.indentLevel = 1;
		
		EditorGUILayout.PropertyField(reelInSpeed, label_reelInSpeed);
		if(reelInSpeed.floatValue<0) reelInSpeed.floatValue = 0;
		EditorGUILayout.PropertyField(payOutSpeed, label_payOutSpeed);
		if(payOutSpeed.floatValue<0) payOutSpeed.floatValue = 0;
		EditorGUI.indentLevel = 0;
		
		EditorGUILayout.LabelField("Player Options",EditorStyles.boldLabel);
		EditorGUILayout.Space();
		EditorGUI.indentLevel = 1;
		
		EditorGUILayout.PropertyField(grapplingHookRange,label_grapplingHookRange);
		if(grapplingHookRange.floatValue<0) grapplingHookRange.floatValue = 0;
		EditorGUILayout.PropertyField(ropeBasePoint,label_ropeBasePoint);
		EditorGUILayout.PropertyField(autoSetLayer,label_autoSetLayer);
		if(!autoSetLayer.boolValue)
		{
			EditorGUI.indentLevel = 2;
			EditorGUILayout.PropertyField(playerLayer,label_playerLayer);
			if(playerLayer.intValue < 0) playerLayer.intValue = 0;
		}
		
		EditorGUI.indentLevel = 0;
		
		EditorGUILayout.LabelField("Physics Options",EditorStyles.boldLabel);
		EditorGUILayout.Space();
		EditorGUI.indentLevel = 1;

		EditorGUILayout.PropertyField(strength , label_strength);
		if(strength.floatValue < 1)
			strength.floatValue =1;

		EditorGUILayout.PropertyField(ropeBendTolerance,label_ropeBendTolerance);
		if(ropeBendTolerance.floatValue<0) ropeBendTolerance.floatValue = 0;
		EditorGUILayout.PropertyField(allowRotation,label_allowRotation);
		EditorGUILayout.PropertyField(ropeCollisions,label_ropeCollisions);
		EditorGUI.indentLevel = 0;
		EditorGUILayout.Space();


		serializedObject.ApplyModifiedProperties();
	}
}
