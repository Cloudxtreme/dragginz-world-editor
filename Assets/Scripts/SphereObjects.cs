//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using System.Collections;

namespace DragginzWorldEditor
{
	public class SphereObjects : MonoBehaviour {

		public int numberOfPoints = 64;
		//public float scale = 3.0f;

		// Use this for initialization
		void Start () {

			GameObject innerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			innerSphere.transform.SetParent(transform);
			innerSphere.transform.position = transform.localPosition;
			innerSphere.transform.localScale = transform.localScale;//innerSphere.transform.localScale * (scale * 2);
			innerSphere.transform.name = "Inner Sphere";

			Vector3[] myPoints = Globals.getPointsOnSphere(numberOfPoints);

			foreach (Vector3 point in myPoints)
			{
				GameObject outerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				outerSphere.transform.SetParent(transform);
				outerSphere.transform.position = transform.localPosition + (point * transform.localScale.x * .5f);
				outerSphere.transform.localScale = transform.localScale * .25f;
			}

		}
	}
}