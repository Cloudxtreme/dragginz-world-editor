using UnityEngine;
using System;

public class FileBrowserDemo : MonoBehaviour
{
	private string m_LabelContent;

	void OnGUI()
	{
		GUI.Label(new Rect(220, 10, Screen.width - 230, Screen.height - 20), m_LabelContent);

		if (GUI.Button(new Rect(10, 10, 200, 50), "Open File"))
		{
			FileBrowser.OpenFilePanel("Open file Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), null, null, (bool canceled, string filePath) => {
				if (canceled)
				{
					m_LabelContent = "[Open File] Canceled";
					Debug.Log("[Open File] Canceled");
					return;
				}

				m_LabelContent = "[Open File] Selected file: " + filePath;
				Debug.Log("[Open File] Selected file: " + filePath);
			});
		}

		if (GUI.Button(new Rect(10, 70, 200, 50), "Open Multiple Files"))
		{
			FileBrowser.OpenMultipleFilesPanel("Open multiple files Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new string[] { "jpg", "png" }, "Open multiple", (bool canceled, string[] filePathArray) => {
				if (canceled)
				{
					m_LabelContent = "[Open Multiple Files] Canceled";
					Debug.Log("[Open Multiple Files] Canceled");
					return;
				}

				m_LabelContent = "";
				for (int i = 0; i < filePathArray.Length; i++)
				{
					m_LabelContent += "[Open Multiple Files] Selected file #" + i + ": " + filePathArray[i] + "\n";
					Debug.Log("[Open Multiple Files] Selected file #" + i + ": " + filePathArray[i]);
				}
			});
		}

		if (GUI.Button(new Rect(10, 130, 200, 50), "Open Folder"))
		{
			FileBrowser.OpenFolderPanel("Open folder Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), null, (bool canceled, string folderPath) => {
				if (canceled)
				{
					m_LabelContent = "[Open Folder] Canceled";
					Debug.Log("[Open Folder] Canceled");
					return;
				}

				m_LabelContent = "[Open Folder] Selected folder: " + folderPath;
				Debug.Log("[Open Folder] Selected folder: " + folderPath);
			});
		}

		if (GUI.Button(new Rect(10, 190, 200, 50), "Open Multiple Folders"))
		{
			FileBrowser.OpenMultipleFoldersPanel("Open multiple folders Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Open folders", (bool canceled, string[] folderPathArray) => {
				if (canceled)
				{
					m_LabelContent = "[Open Multiple Folders] Canceled";
					Debug.Log("[Open Multiple Folders] Canceled");
					return;
				}

				m_LabelContent = "";
				for (int i = 0; i < folderPathArray.Length; i++)
				{
					m_LabelContent += "[Open Multiple Folders] Selected folder #" + i + ": " + folderPathArray[i] + "\n";
					Debug.Log("[Open Multiple Folders] Selected folder #" + i + ": " + folderPathArray[i]);
				}
			});
		}

		if (GUI.Button(new Rect(10, 250, 200, 50), "Save File"))
		{
			FileBrowser.SaveFilePanel("Save file Title", "Type here your message...", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Default Name", new string[] { "jpg", "png" }, null, (bool canceled, string filePath) => {
				if (canceled)
				{
					m_LabelContent = "[Save File] Canceled";
					Debug.Log("[Save File] Canceled");
					return;
				}

				m_LabelContent = "[Save File] You can now save the file to the path: " + filePath;
				Debug.Log("[Save File] You can now save the file to the path: " + filePath);
			});
		}
	}
}
