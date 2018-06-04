#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DWG{
	public static class DWGTagManager {
	
		public static bool TagDefined(string tagName)
		{
			SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty tags = serializedObject.FindProperty("tags");
			tags.Next(true);
			tags.Next(true);
			
			while(tags.Next(false)){
				SerializedProperty tag = tags.Copy();
				if(tag.name == "data"){
					if(tag.stringValue == tagName){
						return true;
					}
				} else {
					return false;
				}
			}
			return false;
		}
		
		public static void AddTag(string destroyTag){
			SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty tagsProperty = serializedObject.FindProperty ("tags");
			tagsProperty.Next(true);
			tagsProperty.arraySize++;
			serializedObject.ApplyModifiedProperties ();
			SerializedProperty tagProperty = tagsProperty.GetArrayElementAtIndex (tagsProperty.arraySize - 1);
			tagProperty.stringValue = destroyTag;
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif