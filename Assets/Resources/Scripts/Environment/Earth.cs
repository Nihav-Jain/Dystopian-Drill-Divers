using UnityEngine;
using System.Collections;

namespace Environment
{
	public class Earth
	{
		public string mMaterialName {get; private set;}
		public float mHardness { get; private set; }

		public Earth(string materialName, float hardness)
		{
			mMaterialName = materialName;
			mHardness = hardness;
		}
	}
}
