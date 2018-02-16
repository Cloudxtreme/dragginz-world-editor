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
		public GameObject go;
		public worldProp(int id, GameObject go) {
			this.id = id;
			this.go = go;
		}
	};

	//
	public class PropsManager : Singleton<PropsManager> {

		private List<propDef> _levelPropDefs;
		private int _iSelectedItem;
		private List<worldProp> _worldProps;

		#region Getters

		public List<propDef> levelPropDefs {
			get { return _levelPropDefs; }
		}

		public int iSelectedItem {
			get { return _iSelectedItem; }
		}

		public List<worldProp> worldProps {
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

			_worldProps = new List<worldProp> ();
		}

		//
		public void addWorldProp(int id, GameObject go) {
			
		}

		//
		public void removeWorldProp(GameObject go) {

		}

		#endregion
	}
}