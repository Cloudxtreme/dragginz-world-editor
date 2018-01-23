using System;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

/// <summary>
/// Has static methods to select/save files and folders on Windows.
/// </summary>
public class FileBrowserWindows
{
	private static string[] m_StringSeparator = new string[] { ",pygmymonkey_separator," };

	private enum DialogType
	{
		OPEN_FILE,
		OPEN_FOLDER,
		SAVE_FILE,
	}

	/// <summary>
	/// Opens the select file panel (Select a single file).
	/// Will only show files with extension defined in extensionArray.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	/// <param name="onDone">Callback called when a file has been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the file selected).</param>
	public static void OpenFilePanel(string title, string startingDirectory, string[] extensionArray, Action<bool, string> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserWindows] There is no callback define for OpenFilePanel, you won't get any result from FileBrowser...");
			return;
		}

		CommonPanel(title, startingDirectory, null, extensionArray, DialogType.OPEN_FILE, false, (bool canceled, string data) => 
		{
			bool isCanceled = canceled || data.Equals("cancel");
			onDone(isCanceled, isCanceled ? null : data);
		});
	}

	/// <summary>
	/// Opens the select multiple files panel (Select multiple files).
	/// Will only show files with extension defined in extensionArray.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	/// <param name="onDone">Callback called when files have been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the file selected array).</param>
	public static void OpenMultipleFilesPanel(string title, string startingDirectory, string[] extensionArray, Action<bool, string[]> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserWindows] There is no callback define for OpenMultipleFilesPanel, you won't get any result from FileBrowser...");
			return;
		}

		CommonPanel(title, startingDirectory, null, extensionArray, DialogType.OPEN_FILE, true, (bool canceled, string data) => 
		{
			string[] dataArray = data.Split(m_StringSeparator, StringSplitOptions.RemoveEmptyEntries);

			bool isCanceled = data.Equals("cancel");
			onDone(isCanceled, isCanceled ? null : dataArray);
		});
	}

	/// <summary>
	/// Opens the select folder panel (Select a single folder).
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="onDone">Callback called when a folder has been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
	public static void OpenFolderPanel(string title, string startingDirectory, Action<bool, string> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserWindows] There is no callback define for OpenFolderPanel, you won't get any result from FileBrowser...");
			return;
		}

		CommonPanel(title, startingDirectory, null, null, DialogType.OPEN_FOLDER, false, (bool canceled, string data) => 
		{
			bool isCanceled = canceled || data.Equals("cancel");
			onDone(isCanceled, isCanceled ? null : data);
		});
	}

	/// <summary>
	/// Opens the select multiple folders panel (Select multiple folders).
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="onDone">Callback called when folders have been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected array).</param>
	public static void OpenMultipleFoldersPanel(string title, string startingDirectory, Action<bool, string[]> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserWindows] There is no callback define for OpenMultipleFoldersPanel, you won't get any result from FileBrowser...");
			return;
		}

		CommonPanel(title, startingDirectory, null, null, DialogType.OPEN_FOLDER, true, (bool canceled, string data) => 
		{
			string[] dataArray = data.Split(m_StringSeparator, StringSplitOptions.RemoveEmptyEntries);

			bool isCanceled = data.Equals("cancel");
			onDone(isCanceled, isCanceled ? null : dataArray);
		});
	}

	/// <summary>
	/// Opens the save file panel (Save a file).
	/// Will set the file types dropdown with the extensions defined in extensionArray, if not null.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="defaultName">Default Name of the file to be saved. (If null, no name is pre-filled in the inputField).</param>
	/// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	/// <param name="onDone">Callback called when a folder has been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
	public static void SaveFilePanel(string title, string startingDirectory, string defaultName, string[] extensionArray, Action<bool, string> onDone)
	{
		CommonPanel(title, startingDirectory, defaultName, extensionArray, DialogType.SAVE_FILE, false, (bool canceled, string data) => 
		{
			bool isCanceled = canceled || data.Equals("cancel");
			onDone(isCanceled, isCanceled ? null : data);
		});
	}

	private static void CommonPanel(string title, string startingDirectory, string defaultName, string[] extensionArray, DialogType dialogType, bool allowMultipleSelection, Action<bool, string> onDone)
	{
		FileBrowserDispatcher.Init();

		if (title == null) title = string.Empty;
		if (startingDirectory == null) startingDirectory = string.Empty;
		startingDirectory = startingDirectory.Replace(@"\\", "/").Replace(@"\", "/");
		if (defaultName == null) defaultName = string.Empty;

		string extensionString = "";
		if (extensionArray != null && extensionArray.Length != 0)
		{
			extensionString = "Files (";

			for (int i = 0; i < extensionArray.Length; i++)
			{
				if (extensionArray[i].Contains(",") || extensionArray[i].Contains(".") || extensionArray[i].Contains("*"))
				{
					Debug.LogError("[FileBrowserWindows] Extensions should not contain , . or *");
					return;
				}

				extensionString += "*." + extensionArray[i] + ", ";
			}

			if (extensionString.EndsWith(", "))
			{
				extensionString = extensionString.Substring(0, extensionString.Length - 2);
			}

			extensionString += ")|";

			for (int i = 0; i < extensionArray.Length; i++)
			{
				extensionString += "*." + extensionArray[i] + ";";
			}
		}

		string fileBrowserExePath = Application.streamingAssetsPath + "/PygmyMonkey/FileBrowser/FileBrowser.exe";
		ThreadStart threadStart = new ThreadStart(() =>
		{
			Process process = new Process();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.Arguments = "/c \""
				+ "\"" + fileBrowserExePath + "\""
				+ " \"" + Process.GetCurrentProcess().Id + "\""
				+ " \"" + title + "\""
				+ " \"" + startingDirectory + "\""
				+ " \"" + defaultName + "\""
				+ " \"" + extensionString + "\""
				+ " \"" + allowMultipleSelection.ToString() + "\""
				+ " \"" + (int)dialogType + "\""
				+ "\"";

			process.Start();
			process.WaitForExit();

			string error = process.StandardError.ReadToEnd();
			if (!string.IsNullOrEmpty(error))
			{
				FileBrowserDispatcher.InvokeAsync(() =>
				{
					error = GetStringFromUnicode(error.Trim());
					Debug.LogError("[FileBrowserWindows] Error:" + error);
					onDone(true, error.Trim());
				});
			}

			string result = process.StandardOutput.ReadToEnd();
			if (!string.IsNullOrEmpty(result) && string.IsNullOrEmpty(error))
			{
				FileBrowserDispatcher.InvokeAsync(() =>
				{
					result = GetStringFromUnicode(result.Trim());
					if (!result.Equals("cancel"))
					{
						result = System.IO.File.ReadAllText(result); // We read the content of the tmp file, containing the result
					}
					
					onDone(false, result);
				});
			}

			process.Close();
		});

		Thread thread = new Thread(threadStart);
		thread.Start();
	}

	private static string GetStringFromUnicode(string data)
	{
		Regex regex = new Regex (@"\\U([0-9A-F]{4})", RegexOptions.IgnoreCase);
		data = regex.Replace(data, match => ((char)int.Parse (match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString());
		return data;
	}
}
