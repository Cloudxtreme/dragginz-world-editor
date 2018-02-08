//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public class Block : MonoBehaviour {

		private World _world;
		private GameObject _parent;

		public void init () {
			_world = World.Instance;
			_parent = transform.parent.gameObject;
		}
		
		void OnBecameVisible () {
			_world.setQuadrantVisibilityFlag (_parent, true);
		}

		void OnBecameInvisible () {
			_world.setQuadrantVisibilityFlag (_parent, false);
		}
	}
}