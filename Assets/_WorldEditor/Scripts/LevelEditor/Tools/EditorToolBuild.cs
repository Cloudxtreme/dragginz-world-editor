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
	public class EditorToolBuild : EditorTool {

		//private Vector3 _v3AimSize;

		private Vector3 _v3BuildSize = Vector3.zero;
		private float _fOffset;

		public EditorToolBuild() : base(Globals.EDITOR_TOOL_BUILD)
		{
			//_v3AimSize = new Vector3 (1, 1, 1);
		}

		//
		public override void deactivate()
		{
			//_v3AimSize = MainMenu.Instance.v3DigSettings;
		}

		//
		public override void activate()
		{
			//MainMenu.Instance.resetDigSettings (_v3AimSize);
		}

		//
		public override void customUpdate(float time, float timeDelta)
		{
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				MainMenu.Instance.toggleDigSize (Input.GetAxis ("Mouse ScrollWheel"));
				//MainMenu.Instance.toggleMaterial (Input.GetAxis ("Mouse ScrollWheel"));
				//setCurAimCenterCubeMaterial();
			}

			doRayCast ();

			if (_goHit != null)
			{
				if (_rendererAimCenterCube.sharedMaterial == _materialAimTool) {
					setCurAimCenterCubeMaterial();
				}

				_v3BuildSize = MainMenu.Instance.v3DigSettings;

				_v3Pos = _goHit.transform.position + (_hit.normal * _levelEditor.fRockSize);
				_trfmAimTool.forward  = _hit.normal;

				if ((int)_v3BuildSize.x > 1) {
					_fOffset = 0.5f * ((_v3BuildSize.x - 1) * _levelEditor.fRockSize);
					_v3Pos -= _fOffset * _trfmAimTool.right;
				}
				if ((int)_v3BuildSize.y > 1) {
					_fOffset = 0.5f * ((_v3BuildSize.y - 1) * _levelEditor.fRockSize);
					_v3Pos += _fOffset * _trfmAimTool.up;
				}
				if ((int)_v3BuildSize.z > 1) {
					_fOffset = 0.5f * ((_v3BuildSize.z - 1) * _levelEditor.fRockSize);
					_v3Pos += _fOffset * _trfmAimTool.forward;
				}

				_trfmAimTool.position = _v3Pos;

				_trfmAimCenterCube.position = _goHit.transform.position + (_hit.normal * _levelEditor.fRockSize);

				if (_mouseIsDown) {
					buildIt (_trfmAimCenterCube.position);
					_mouseIsDown = false;
				}
			}
			else {
				resetAim ();
			}
		}

		private void buildIt(Vector3 v3Pos)
		{
			Vector3 v3BlockPos = v3Pos;
			float fAdd = 0;

			_levelEditor.resetUndoActions ();

			for (int x = 0; x < (int)_v3BuildSize.x; x++) {
				for (int y = 0; y < (int)_v3BuildSize.y; y++) {
					for (int z = 0; z < (int)_v3BuildSize.z; z++) {

						v3BlockPos = v3Pos;

						fAdd = (x * _levelEditor.fRockSize);
						v3BlockPos -=  (fAdd * _trfmAimTool.right);

						fAdd = (y * _levelEditor.fRockSize);
						v3BlockPos +=  (fAdd * _trfmAimTool.up);

						fAdd = (z * _levelEditor.fRockSize);
						v3BlockPos +=  (fAdd * _trfmAimTool.forward);

						buildBlock (v3BlockPos);
					}
				}
			}
		}

		private void buildBlock(Vector3 v3Pos)
		{
			int x = (int)(v3Pos.x < 0 ? Math.Round (v3Pos.x, MidpointRounding.AwayFromZero) : v3Pos.x);
			int y = (int)(v3Pos.y < 0 ? Math.Round (v3Pos.y, MidpointRounding.AwayFromZero) : v3Pos.y);
			int z = (int)(v3Pos.z < 0 ? Math.Round (v3Pos.z, MidpointRounding.AwayFromZero) : v3Pos.z);

			// get quadrant

			Vector3 v3QuadrantPos = new Vector3 ((float)x / 1f, (float)y / 1f, (float)z / 1f);
			string quadrantId = (int)v3QuadrantPos.x + "_" + (int)v3QuadrantPos.y + "_" + (int)v3QuadrantPos.z;
			string sQuadrantName = Globals.containerGameObjectPrepend + quadrantId;
			Transform trfmQuadrant = _levelEditor.curLevelChunk.trfmCubes.Find (sQuadrantName);

			//Debug.Log ("quadrant: "+trfmQuadrant+" - "+trfmQuadrant.name);
			if (trfmQuadrant == null) {
				Debug.LogError ("quadrant " + sQuadrantName + " not found!");
				return;
			}

			// get cild

			Vector3 v3LocalBlockPos = new Vector3 (
				Mathf.Abs(v3QuadrantPos.x-v3Pos.x),
				Mathf.Abs(v3QuadrantPos.y-v3Pos.y),
				Mathf.Abs(v3QuadrantPos.z-v3Pos.z)
			);

			int len = _levelEditor.cubesPerQuadrant;
			int id = ((int)(v3LocalBlockPos.x / _levelEditor.fRockSize)) * (len * len);
			id += ((int)(v3LocalBlockPos.y / _levelEditor.fRockSize)) * len;
			id += ((int)(v3LocalBlockPos.z / _levelEditor.fRockSize));

			Transform container = trfmQuadrant.Find ("container");
			Transform trfmChild = container.Find (id.ToString ());
			if (trfmChild != null)
			{
				_levelEditor.addUndoAction (AppState.Build, trfmChild.gameObject);
				trfmChild.gameObject.SetActive (true);
				setSingleMaterial (trfmChild.gameObject, _levelEditor.aMaterials[MainMenu.Instance.iSelectedMaterial], false);
				//Debug.LogWarning ("child "+id.ToString ()+" exists!");
			}
			/*else {
				GameObject goNew = World.Instance.createRock (v3LocalBlockPos, container.gameObject, id.ToString ());
				setSingleMaterial (goNew, _levelEditor.aMaterials[MainMenu.Instance.iSelectedMaterial], false);
				_levelEditor.resetUndoActions ();
				_levelEditor.addUndoAction (AppState.Build, goNew);
			}*/
		}
	}
}