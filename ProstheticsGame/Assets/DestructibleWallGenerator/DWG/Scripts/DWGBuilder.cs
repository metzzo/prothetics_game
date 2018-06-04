#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DWG {
	public static class DWGBuilder {
	
		public static GameObject destructibleObject, finalPosition;
		public static int createPrefab, upAxis, colliderType, length, height, sides;
		public static bool hasPhysics, checkPhysics, _singleCollider;
		public static Vector3 createPrefabSize;
		public static Material createPrefabMat;
		
		private static GameObject newWall;
		private static Vector3 objectScale;
		private static Bounds meshSize;
		
		public static void Build(){
			if(destructibleObject){
				objectScale = destructibleObject.transform.localScale; 
			} else {
				objectScale = new Vector3(1,1,1);
			}
			Generate(Wall());
	    }
	    
		static void Generate(GameObject newWall){
			float newRot = 0; // Set a float for a new rotation angle
			Vector3 startPos = newWall.transform.position; // Create a new starting position based on the newWall game object
			
			for(int i= 0; i < sides; i++){
				if(i == 0){
					GameObject northWall = NorthWall(newWall); // Create a new game object using the NorthWall() returned game object
					BuildWall(newWall.transform.position,northWall); // Call up the BuildWall function using the newWall position and the northWall game object
					CenterOfChildren(northWall.transform);
					AddColliders(northWall.transform); // Call the add colliders function for the northWall
				} else {
					newRot += 90; 
					float meshX = meshSize.size.x * objectScale.x; 
					float meshZ;
					if(upAxis == 0){
						meshZ = meshSize.size.y * objectScale.y; 
					}else{
						meshZ = meshSize.size.z * objectScale.z; 
					}
	
					Vector3 newPos = Vector3.zero; // Set a new vector3
					GameObject newSide = new GameObject(); // Create a new empty game object
					
					if(i==1){
						newSide.gameObject.name = "EastWall"; // Name the object East Wall
						newPos = new Vector3((meshX * length) - ((meshX / 2) + (meshZ / 2)),0, 
											 -((meshX / 2) + (meshZ / 2)));
					} else if(i==2){
						newSide.gameObject.name = "SouthWall";// Name the object South Wall
						newPos = new Vector3(-((meshX / 2) + (meshZ / 2)),0,
											 -(meshX * length) + ((meshX / 2) + (meshZ / 2)));
					} else if(i==3){
						newSide.gameObject.name = "WestWall";// Name the object West Wall
						newPos = new Vector3(-(meshX * length) + ((meshX / 2) + (meshZ / 2)),0,
											 (meshX / 2) + (meshZ / 2));
	                }
	                
	            
	                startPos += newPos; // adds the start position and the new position together
	                newSide.transform.parent = newWall.transform; // sets the new game object 
	                newSide.transform.position = startPos; // newSide game object position is set to the start position that the above equation handled
					BuildWall(startPos, newSide); // Build a new wall! using the start position and the new game object
					newSide.transform.localEulerAngles = new Vector3(0,newRot,0); // Oh yeah, and rotate that new wall based on the newRot
					CenterOfChildren(newSide.transform);
	                AddColliders(newSide.transform); // Add some colliders to the bricks you just made (which was done in BuildWall()
	            }
	        }
			CenterOfChildren(newWall.transform);
			
			if(finalPosition != null){
				newWall.transform.position = finalPosition.transform.position;
			} else {
				newWall.transform.position = Vector3.zero;
				if(upAxis == 0){
					newWall.transform.position += new Vector3(0,meshSize.extents.z * height,0);
				} else {
					newWall.transform.position += new Vector3(0,meshSize.extents.y * height,0);
				}
			}
	    }
	    
		static GameObject Wall(){
			newWall = new GameObject(); // Create an empty game object
			newWall.name = "Wall"; // Name the emtpy game object Wall
			newWall.transform.position = Vector3.zero; // Set the position equaled to the starting point of your wall
			return newWall; // Return the newly created empty game object named Wall
		}
		static GameObject NorthWall(GameObject newWall){
			GameObject northWall = new GameObject(); /// Create an empty game object
			northWall.name = "NorthWall"; // Name the empty game object NorthWall
			northWall.transform.position = newWall.transform.position; // Set the position equaled the game object value that was carried over to this function (newWall)
			northWall.transform.parent = newWall.transform; // Set the newly created empty game objects parent to the object that was carried over (newWall)
			return northWall; // Return the newly created empty game object named NorthWall
		}
	    
		static void BuildWall(Vector3 nextPos, GameObject newSide){
			for(int i = 0; i < height; i++){
				if(i > 0){
					Vector3 newPos = Vector3.zero; // Set a new vector3 
					if(upAxis == 0){
						newPos = new Vector3(meshSize.extents.x * objectScale.x,meshSize.size.z * objectScale.z,0);
					} else {
						newPos = new Vector3(meshSize.extents.x * objectScale.x,meshSize.size.y * objectScale.y,0);
					}
					if(i % 2 == 0){
						nextPos += newPos;
					} else {
						nextPos.x -= newPos.x;
						nextPos.y += newPos.y;
					}
				}
				AddBricks(nextPos, newSide);
			}
		}
		
		static void AddBricks(Vector3 nextPos, GameObject newSide)
		{
			for(int i = 0; i < length; i++){
				GameObject newPiece;
				if(createPrefab == 0){
					newPiece = PrefabUtility.InstantiatePrefab(destructibleObject) as GameObject;
				} else {
					newPiece = new GameObject();
					DWGCreateBrick.CreateBrick(newPiece,createPrefabSize.x,createPrefabSize.y,createPrefabSize.z);
					if(createPrefabMat){
						newPiece.GetComponent<MeshRenderer>().material = createPrefabMat;
					} else {
						newPiece.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
					}
				}
				newPiece.name = "Brick";
				newPiece.transform.position = nextPos;
				// ^- Instantiate your brick prefab at the nextPos using the prefabs rotation
				newPiece.transform.parent = newSide.transform; // Set the instantiated objects parent to be thew newSide game object
				meshSize = newPiece.GetComponent<MeshFilter>().sharedMesh.bounds; // Set the meshSize Bounds to equal the mesh of the instantiated object
				Vector3 newPosX = new Vector3(meshSize.size.x * objectScale.x,0,0); // Set a new vector3 to have its X the same as the meshSize X * the scale
				nextPos += newPosX; // Next position is set to next position plus the newpositionX. this is so the next brick will be placed beside this one
			}
		}
		
		static void AddColliders(Transform parent){
			if(_singleCollider){
				if(parent.GetComponent<Collider>()){
					GameObject.DestroyImmediate(parent.GetComponent<Collider>());
				}
				foreach(Transform child in parent){
					if(child.GetComponent<Collider>())
						GameObject.DestroyImmediate(child.GetComponent<Collider>());
					}
				parent.gameObject.AddComponent<BoxCollider>();
				BoxCollider boxCol = parent.gameObject.GetComponent<BoxCollider>();
				
				if(upAxis == 0){
					boxCol.size = new Vector3(((meshSize.size.x * length) + meshSize.extents.x) * objectScale.x,
				    	                      (meshSize.size.z * height) * objectScale.z,
				        	                  meshSize.size.y * objectScale.y);
				} else {
					boxCol.size = new Vector3(((meshSize.size.x * length) + meshSize.extents.x) * objectScale.x,
					                          (meshSize.size.y * height) * objectScale.y,
					                          meshSize.size.z * objectScale.z);
				}
				
				if(colliderType != 0) {
					parent.gameObject.GetComponent<Collider>().enabled = false; // Disable colliders
					parent.gameObject.AddComponent<DWGColliderEnabler>(); // This adds a collider enabler to the brick. This way the collider only enables when the game is in play. That way you can move objects in the scene view without delay
				}
			} else {
		    	foreach(Transform child in parent){
					if(child.GetComponent<Collider>()){
						GameObject.DestroyImmediate(child.GetComponent<Collider>()); // If there's a collider, then destroy it before applying the propper collider
		        	}
		       		if(colliderType == 1){
						child.gameObject.AddComponent<BoxCollider>(); // this adds a box collider to the child object
					}
					else if(colliderType == 2){
						child.gameObject.AddComponent<SphereCollider>(); // this adds a sphere collider to the child object
					}
					else if(colliderType == 3){
						child.gameObject.AddComponent<MeshCollider>(); // this adds a mesh collider to the child object
						child.gameObject.GetComponent<MeshCollider>().convex = true; // This turns on the convex on which allows collision with other colliders
		        	}
			        if(hasPhysics) {
						string destroyTag = "Destructible";
						if(!DWGTagManager.TagDefined(destroyTag)){
							DWGTagManager.AddTag(destroyTag);
						}
						child.gameObject.tag = destroyTag;
						
			            child.gameObject.AddComponent<Rigidbody>(); // This adds a rigidbody to the child game object
			            child.GetComponent<Rigidbody>().isKinematic = true; // Set isKinematic to true so that physics aren't enabled on the child object
			            if(checkPhysics){
							child.gameObject.AddComponent<DWGPhysicsCheck>(); // Add the PhysicsCheck script (this checks to see if the rigidbody is asleep, if it is, it turns kinematic to true)
						}
			        }
			        if(colliderType != 0) {
			        	child.gameObject.GetComponent<Collider>().enabled = false; // Disable colliders
			        	child.gameObject.AddComponent<DWGColliderEnabler>(); // This adds a collider enabler to the brick. This way the collider only enables when the game is in play. That way you can move objects in the scene view without delay
			        }
		        }
	        }
	    }
	    
		static void CenterOfChildren(Transform parent){
			List<Transform> children = parent.Cast<Transform>().ToList();
			Vector3 pos = Vector3.zero;
			foreach(Transform child in children){
				pos += child.position;
				child.parent = null;
			}
			pos /= children.Count;
			parent.position = pos;
			foreach(Transform child in children){
				child.parent = parent;
			}
		}
	}
}
#endif