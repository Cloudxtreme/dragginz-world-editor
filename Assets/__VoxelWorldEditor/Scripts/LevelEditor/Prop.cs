//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using RTEditor;

namespace DragginzVoxelWorldEditor
{
	public class Prop : MonoBehaviour, IRTEditorEventListener {

		/// <summary>
		/// Called before an object is about to be selected. Must return true if the
		/// object can be selected and false otherwise.
		/// </summary>
		public bool OnCanBeSelected(ObjectSelectEventArgs selectEventArgs)
		{
			Debug.LogWarning ("OnCanBeSelected "+name);

			if (AppController.Instance.appState != AppState.Select) {
				return false;
			}

			EditorObjectSelection selection = EditorObjectSelection.Instance;

			Transform trfmParent = transform.parent;
			while (trfmParent != null && trfmParent != LevelEditor.Instance.curLevelChunk.trfmProps) {
				selection.AddObjectToSelection(trfmParent.gameObject, false);
				Debug.Log ("    ->"+trfmParent.name);
				trfmParent = trfmParent.parent;
			}

			return true;
		}

		/// <summary>
		/// Called when the object has been selected.
		/// </summary>
		public void OnSelected(ObjectSelectEventArgs selectEventArgs)
		{
			Debug.LogWarning ("OnSelected "+name);
		}

		/// <summary>
		/// Called when the object has been deselected.
		/// </summary>
		public void OnDeselected(ObjectDeselectEventArgs deselectEventArgs)
		{
			Debug.LogWarning ("OnDeselected "+name+": "+deselectEventArgs.DeselectActionType);
		}

		/// <summary>
		/// Called when the object is altered (moved, rotated or scaled) by a transform gizmo.
		/// </summary>
		/// <param name="gizmo">
		/// The transform gzimo which alters the object.
		/// </param>
		public void OnAlteredByTransformGizmo(Gizmo gizmo)
		{
			//
		}
	}
}