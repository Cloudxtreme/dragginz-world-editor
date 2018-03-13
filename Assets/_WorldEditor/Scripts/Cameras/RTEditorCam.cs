//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;

namespace DragginzWorldEditor
{
	public class RTEditorCam : MonoSingleton<FlyCam> {

		public bool drawWireframe;

		void Awake() {
			drawWireframe = false;
		}

		void OnPreRender() {
			GL.wireframe = drawWireframe;
		}
		void OnPostRender() {
			GL.wireframe = false;
		}
	}
}