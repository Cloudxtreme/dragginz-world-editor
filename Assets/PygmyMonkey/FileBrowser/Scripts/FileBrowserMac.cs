using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

/// <summary>
/// Has static methods to select/save files and folders on Mac.
/// </summary>
public class FileBrowserMac
{
	private static string[] m_StringSeparator = new string[] { ",pygmymonkey_separator," };
	private static Action<bool, string> m_SingleCallback;
	private static Action<bool, string[]> m_MultipleCallback;

	private delegate void CallbackFunction(string data);

	[DllImport("FileBrowserBundle")]
	private static extern void _OpenPanel(IntPtr startingDirectory, IntPtr extension, IntPtr buttonName, bool allowDirectories, bool allowFiles, bool allowMultipleSelection, CallbackFunction callback);

	[DllImport("FileBrowserBundle")]
	private static extern void _SavePanel(IntPtr message, IntPtr startingDirectory, IntPtr defaultName, IntPtr extension, IntPtr buttonName, CallbackFunction callback);

	private class MonoPInvokeCallbackAttribute : Attribute
	{
		public Type type;
		public MonoPInvokeCallbackAttribute(Type t) { type = t; }
	}

	/// <summary>
	/// Opens the select file panel (Select a single file).
	/// Will only allow to select files with extension defined in extensionArray.
	/// </summary>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	/// <param name="buttonName">The name of the button. You can set this to null to use the defaut value.</param>
	/// <param name="onDone">Callback called when a file has been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the file selected).</param>
	public static void OpenFilePanel(string startingDirectory, string[] extensionArray, string buttonName, Action<bool, string> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserMac] There is no callback define for OpenFilePanel, you won't get any result from FileBrowser...");
			return;
		}

		m_SingleCallback = onDone;
		CommonOpenPanel(startingDirectory, extensionArray, buttonName, false, true, false);
	}

	/// <summary>
	/// Opens the select multiple files panel (Select multiple files).
	/// Will only allow to select files with extension defined in extensionArray.
	/// </summary>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	/// <param name="buttonName">The name of the button. You can set this to null to use the defaut value.</param>
	/// <param name="onDone">Callback called when files have been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the file selected array).</param>
	public static void OpenMultipleFilesPanel(string startingDirectory, string[] extensionArray, string buttonName, Action<bool, string[]> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserMac] There is no callback define for OpenMultipleFilesPanel, you won't get any result from FileBrowser...");
			return;
		}

		m_MultipleCallback = onDone;
		CommonOpenPanel(startingDirectory, extensionArray, buttonName, false, true, true);
	}

	/// <summary>
	/// Opens the select folder panel (Select a single folder).
	/// </summary>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="buttonName">The name of the button. You can set this to null to use the defaut value.</param>
	/// <param name="onDone">Callback called when a folder has been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
	public static void OpenFolderPanel(string startingDirectory, string buttonName, Action<bool, string> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserMac] There is no callback define for OpenFolderPanel, you won't get any result from FileBrowser...");
			return;
		}

		m_SingleCallback = onDone;
		CommonOpenPanel(startingDirectory, null, buttonName, true, false, false);
	}

	/// <summary>
	/// Opens the select multiple folders panel (Select multiple folders).
	/// </summary>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="buttonName">The name of the button. You can set this to null to use the defaut value.</param>
	/// <param name="onDone">Callback called when folders have been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected array).</param>
	public static void OpenMultipleFoldersPanel(string startingDirectory, string buttonName, Action<bool, string[]> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserMac] There is no callback define for OpenMultipleFoldersPanel, you won't get any result from FileBrowser...");
			return;
		}

		m_MultipleCallback = onDone;
		CommonOpenPanel(startingDirectory, null, buttonName, true, false, true);
	}

	private static void CommonOpenPanel(string startingDirectory, string[] extensionArray, string buttonName, bool allowDirectories, bool allowFiles, bool allowMultipleSelection)
	{
		if (startingDirectory == null) startingDirectory = string.Empty;

		string extensionString = "null";
		if (extensionArray != null)
		{
			for (int i = 0; i < extensionArray.Length; i++)
			{
				if (extensionArray[i].Contains(",") || extensionArray[i].Contains(".") || extensionArray[i].Contains("*"))
				{
					Debug.LogError("[FileBrowserMac] Extensions should not contain , . or *");
					return;
				}
			}

			extensionString = string.Join(",", extensionArray);
		}

		if (string.IsNullOrEmpty(buttonName)) buttonName = "null";

		_OpenPanel(Marshal.StringToHGlobalAuto(startingDirectory), Marshal.StringToHGlobalAuto(extensionString), Marshal.StringToHGlobalAuto(buttonName), allowDirectories, allowFiles, allowMultipleSelection, ReceiveDeviceMessage);
	}

	/// <summary>
	/// Opens the save file panel (Save a file).
	/// Will only allow to save the file with the extension(s) defined in extensionArray, if not null.
	/// </summary>
	/// <param name="message">A hint message on top of the panel, to display a hint to users.</param>
	/// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	/// <param name="defaultName">Default Name of the file to be saved. (If null, no name is pre-filled in the inputField).</param>
	/// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	/// <param name="buttonName">The name of the button. You can set this to null to use the defaut value.</param>
	/// <param name="onDone">Callback called when a folder has been chosen (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
	public static void SaveFilePanel(string message, string startingDirectory, string defaultName, string[] extensionArray, string buttonName, Action<bool, string> onDone)
	{
		if (onDone == null)
		{
			Debug.LogError("[FileBrowserMac] There is no callback define for SaveFilePanel, you won't get any result from FileBrowser...");
			return;
		}

		m_SingleCallback = onDone;

		if (message == null) message = string.Empty;
		if (startingDirectory == null) startingDirectory = string.Empty;
		if (string.IsNullOrEmpty(defaultName)) defaultName = "null";

		string extensionString = "null";
		if (extensionArray != null)
		{
			for (int i = 0; i < extensionArray.Length; i++)
			{
				if (extensionArray[i].Contains(",") || extensionArray[i].Contains(".") || extensionArray[i].Contains("*"))
				{
					Debug.LogError("[FileBrowserMac] Extensions should not contain , . or *");
					return;
				}
			}

			extensionString = string.Join(",", extensionArray);
		}

		if (string.IsNullOrEmpty(buttonName)) buttonName = "null";

		_SavePanel(Marshal.StringToHGlobalAuto(message), Marshal.StringToHGlobalAuto(startingDirectory), Marshal.StringToHGlobalAuto(defaultName), Marshal.StringToHGlobalAuto(extensionString), Marshal.StringToHGlobalAuto(buttonName), ReceiveDeviceMessage);
	}

	[MonoPInvokeCallback(typeof(CallbackFunction))]
	private static void ReceiveDeviceMessage(string data) 
	{
		try
		{
			if (m_SingleCallback != null)
			{
				bool isCanceled = data.Equals("cancel");
				m_SingleCallback(isCanceled, isCanceled ? null : data);
			}
			
			if (m_MultipleCallback != null)
			{
				string[] dataArray = data.Split(m_StringSeparator, StringSplitOptions.RemoveEmptyEntries);

				bool isCanceled = dataArray.Contains("cancel");
				m_MultipleCallback(isCanceled, isCanceled ? null : dataArray);
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("[FileBrowserMac] An exception occured: " + exception);
		}

		m_SingleCallback = null;
		m_MultipleCallback = null;
	}
}
