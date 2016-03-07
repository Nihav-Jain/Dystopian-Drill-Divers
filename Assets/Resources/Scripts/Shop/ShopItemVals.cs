using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

namespace Shop
{
	//[System.Serializable]
	public class ShopItemVals : MonoBehaviour
	{
		public int[] itemCost;
		public int[] itemValue;
		public Image[] itemImage;
	}

	//[CustomEditor(typeof(ShopItemVals))]
	//public class CustomShopItemEditor : Editor
	//{
	//	public override void OnInspectorGUI()
	//	{
	//		serializedObject.Update();
	//		var controller = (ShopItemVals)target;
	//		EditorGUIUtility.LookLikeInspector();
	//		SerializedProperty tps = serializedObject.FindProperty("targetPoints");
	//		EditorGUI.BeginChangeCheck();
	//		EditorGUILayout.PropertyField(tps, true);
	//		if (EditorGUI.EndChangeCheck())
	//			serializedObject.ApplyModifiedProperties();
	//		EditorGUIUtility.LookLikeControls();
	//		// ...
	//	}
	//}
}
