//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using System.Collections;

namespace DragginzWorldEditor
{
	[CreateAssetMenu(fileName = "NewProp", menuName = "Dragginz/Prop", order = 1)]
	public class PropDefinition : ScriptableObject
	{
		public string propName     = "New Prop";
		public GameObject prefab   = null;
		public bool isUsingGravity = true;
	}
}