﻿//
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
	public class LevelObject {

		[SerializeField]
		public string name { get; set; }

		//public DataTypeVector3[] vertices { get; set; }
		//public int[] triangles { get; set; }
		//public DataTypeVector3[] normals { get; set; }
		//public DataTypeVector2[] uv { get; set; }

		[SerializeField]
		public DataTypeVector3 position  { get; set; }
		//public DataTypeQuaternion rotation  { get; set; }
		//public DataTypeVector3 scale { get; set; }

		[SerializeField]
		public string material { get; set; }
	}
}