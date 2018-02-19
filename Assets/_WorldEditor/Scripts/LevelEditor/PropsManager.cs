//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public struct propDef {
		public int id;
		public string name;
		public GameObject prefab;
		public bool useCollider;
		public bool useGravity;
	};

	public struct worldProp {
		public int id;
		public string name; 
		public GameObject go;
		public worldProp(int id, string name, GameObject go) {
			this.id = id;
			this.name = name;
			this.go = go;
		}
	};

	//
	public class PropsManager : Singleton<PropsManager>
	{
		private List<propDef> _levelPropDefs;
		private int _iSelectedItem;
		private Dictionary<GameObject, worldProp> _worldProps;

		#region Getters

		public List<propDef> levelPropDefs {
			get { return _levelPropDefs; }
		}

		public int iSelectedItem {
			get { return _iSelectedItem; }
		}

		public Dictionary<GameObject, worldProp> worldProps {
			get { return _worldProps; }
		}

		#endregion

		#region PublicMethods

		public void init()
		{
			_levelPropDefs = new List<propDef> ();

			PropsList propList = Resources.Load<PropsList> ("Data/" + Globals.propListName);
			int i, len = propList.props.Count;
			for (i = 0; i < len; ++i) {

				PropDefinition propDef = propList.props [i];
				if (propDef.prefab != null) {

					propDef p   = new propDef ();
					p.id          = propDef.id;
					p.name        = propDef.propName;
					p.prefab      = propDef.prefab;
					p.useCollider = propDef.isUsingCollider;
					p.useGravity  = propDef.isUsingGravity;

					_levelPropDefs.Add (p);
				}
			}

			_iSelectedItem = 0;

			_worldProps = new Dictionary<GameObject, worldProp> ();
		}

		//
		public propDef getSelectedPropDef()
		{
			return _levelPropDefs [_iSelectedItem];
		}

		//
		public propDef getPropDefForId(int id)
		{
			propDef p = new propDef();
			p.id = -1;

			int i, len = _levelPropDefs.Count;
			for (i = 0; i < len; ++i) {
				if (_levelPropDefs [i].id == id) {
					p = _levelPropDefs [i];
					break;
				}
			}

			return p;
		}

		//
		public propDef getPropDefForGameObject(GameObject go)
		{
			propDef p = new propDef();
			p.id = -1;

			if (_worldProps.ContainsKey (go)) {
				p = getPropDefForId(_worldProps [go].id);
			}

			return p;
		}

		//
		public void reset()
		{
			_worldProps.Clear ();
		}

		//
		public void toggleSelectedProp(float toggle)
		{
			if (toggle < 0) {
				_iSelectedItem = (_iSelectedItem > 0 ? _iSelectedItem - 1 : 0);
			} else {
				_iSelectedItem = (_iSelectedItem < (_levelPropDefs.Count - 1) ? _iSelectedItem + 1 : (_levelPropDefs.Count - 1));
			}

			LevelEditor.Instance.newItemSelected (_iSelectedItem);
		}

		//
		public void addWorldProp(int id, GameObject go)
		{
			_worldProps.Add (go, new worldProp (id, go.name, go));
		}

		//
		public void removeWorldProp(GameObject go)
		{
			if (_worldProps.ContainsKey (go)) {
				_worldProps.Remove (go);
			}
		}

		#endregion
	}
}