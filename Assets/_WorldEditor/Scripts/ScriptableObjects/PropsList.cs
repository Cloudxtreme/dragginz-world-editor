//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DragginzWorldEditor
{
	[CreateAssetMenu(fileName = "NewPropsList", menuName = "Dragginz/Props List", order = 2)]
	public class PropsList : ScriptableObject
	{
		public List<PropDefinition> props;
	}
}