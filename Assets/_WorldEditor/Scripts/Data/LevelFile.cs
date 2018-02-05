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
	public class LevelFile {

		[SerializeField]
		public int fileFormatVersion { get; set; }

		[SerializeField]
		public string levelName { get; set; }

		[SerializeField]
		public List<LevelQuadrant> levelQuadrants { get; set; }
	}
}