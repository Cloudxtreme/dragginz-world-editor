//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using System.Collections;

namespace AssetsShared
{
	[CreateAssetMenu(fileName = "NewProp", menuName = "Dragginz/Prop", order = 1)]
	public class PropDefinition : ScriptableObject
	{
		public int id              = 0;
		public string propName     = "New Prop";

		public GameObject prefab   = null;

		public bool isUsingCollider = true;
		public bool isUsingGravity  = true;
	}
}