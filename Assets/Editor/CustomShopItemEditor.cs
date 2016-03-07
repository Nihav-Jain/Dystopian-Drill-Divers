using UnityEngine;
using System.Collections;
using Shop;
using UnityEditor;

[CustomEditor(typeof(ShopItemVals))]
public class CustomShopItemEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("itemCost"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("itemValue"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("itemImage"), true);
		serializedObject.ApplyModifiedProperties();
		// ...
	}
}
