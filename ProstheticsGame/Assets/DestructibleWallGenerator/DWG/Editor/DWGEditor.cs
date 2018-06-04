#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using DWG;

namespace DWG{
	public class DWGEditor : EditorWindow {
	
		[MenuItem ("DWG/Generator")]
		static void Init () {
			DWGEditor window = (DWGEditor)EditorWindow.GetWindow (typeof (DWGEditor));
			window.title = "DWG";
			window.maxSize = new Vector2(300,250);
			window.minSize = new Vector2(300,250);
		}
	
		private string[] createPrefabStr = new string[]{"Use Prefab", "Create Brick"};
		private string[] colTypeStr = new string[]{"None","Box","Sphere","Mesh"}; 
		private string[] upAxisStr = new string[]{"Z","Y"}; 
	
		void OnGUI()
		{
			DWGBuilder.createPrefab = GUILayout.SelectionGrid(DWGBuilder.createPrefab,createPrefabStr , 2);
			
			if(DWGBuilder.createPrefab == 1){
				DWGBuilder.upAxis = 1;
				DWGBuilder.createPrefabMat = (Material)EditorGUILayout.ObjectField("Material", DWGBuilder.createPrefabMat,typeof(Material),false);
				DWGBuilder.createPrefabSize.x =EditorGUILayout.Slider("Brick Width",DWGBuilder.createPrefabSize.x,0.01f,1.00f);
				DWGBuilder.createPrefabSize.y =EditorGUILayout.Slider("Brick Height",DWGBuilder.createPrefabSize.y,0.01f,1.00f);
				DWGBuilder.createPrefabSize.z = DWGBuilder.createPrefabSize.x / 2;
			} else {
					DWGBuilder.destructibleObject = (GameObject)EditorGUILayout.ObjectField("Brick Prefab",DWGBuilder.destructibleObject,typeof(GameObject),true); // Basic object field to add prefabs to
					DWGBuilder.upAxis = EditorGUILayout.Popup("Up Axis", DWGBuilder.upAxis, upAxisStr); 
					
			}
			DWGBuilder.colliderType = EditorGUILayout.Popup("Collider Type", DWGBuilder.colliderType, colTypeStr); 
			EditorGUI.indentLevel = 10;
			if(DWGBuilder.colliderType == 1 && !DWGBuilder.hasPhysics){
				DWGBuilder._singleCollider = EditorGUILayout.ToggleLeft("Single Collider", DWGBuilder._singleCollider);	
			} else {
				DWGBuilder._singleCollider = false;
			}
			if(DWGBuilder.colliderType != 0 && !DWGBuilder._singleCollider) {
				DWGBuilder.hasPhysics = EditorGUILayout.ToggleLeft("Add Physics", DWGBuilder.hasPhysics); 
			} else {
				DWGBuilder.hasPhysics = false;
			}
			if(DWGBuilder.colliderType != 0 && DWGBuilder.hasPhysics){
				DWGBuilder.checkPhysics = EditorGUILayout.ToggleLeft("Kinematic on Sleep", DWGBuilder.checkPhysics);
			}
			EditorGUI.indentLevel = 0;
			
			DWGBuilder.finalPosition = (GameObject)EditorGUILayout.ObjectField("Wall Position",DWGBuilder.finalPosition,typeof(GameObject),true);
			DWGBuilder.length = EditorGUILayout.IntSlider("Length",DWGBuilder.length,1,25);
			DWGBuilder.height = EditorGUILayout.IntSlider("Height",DWGBuilder.height,1,25);
			DWGBuilder.sides = EditorGUILayout.IntSlider("Sides",DWGBuilder.sides,1,4);
			Mathf.Clamp(DWGBuilder.sides,1,4);
			if(GUILayout.Button("Generate")){
				if(DWGBuilder.createPrefab == 0 && !DWGBuilder.destructibleObject){
					EditorUtility.DisplayDialog("Nope!","Make sure you have a prefab selected!", "OK");
				} else {
					DWGBuilder.Build ();
				}
			}
		}
	}
}
#endif