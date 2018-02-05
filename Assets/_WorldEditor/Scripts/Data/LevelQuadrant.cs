//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzWorldEditor
{
	[Serializable]
	public class LevelQuadrant {

		[SerializeField]
		public string name { get; set; }

		[SerializeField]
		public DataTypeVector3 position  { get; set; }

		[SerializeField]
		public List<LevelObject> levelObjects { get; set; }

	}
}