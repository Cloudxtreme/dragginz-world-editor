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

		private LevelEditor _levelEditor;
		private GameObject _parent;

		//private MeshRenderer _meshRenderer;
		//private BoxCollider _boxCollider;

		public void init () {
			_levelEditor = LevelEditor.Instance;
			_parent = transform.parent.gameObject;
			//_meshRenderer = GetComponent<MeshRenderer> ();
			//_boxCollider = GetComponent<BoxCollider> ();
		}
		
		void OnBecameVisible () {
			_levelEditor.setQuadrantVisibilityFlag (_parent, true);
			//_meshRenderer.enabled = true;
			//_boxCollider.enabled = true;
		}

		void OnBecameInvisible () {
			_levelEditor.setQuadrantVisibilityFlag (_parent, false);
			//_meshRenderer.enabled = false;
			//_boxCollider.enabled = false;
		}
	}
}