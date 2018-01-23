using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

/// Adapted from: https://github.com/nickgravelyn/UnityToolbag/tree/master/Dispatcher
/// <summary>
/// A system for dispatching code to execute on the main thread.
/// </summary>
[AddComponentMenu("UnityToolbag/Dispatcher")]
public class FileBrowserDispatcher : MonoBehaviour
{
	private static FileBrowserDispatcher m_Instance;

	// We can't use the behaviour reference from other threads, so we use a separate bool
	// to track the instance so we can use that on the other threads.
	private static Thread m_MainThread;
	private static object m_LockObject = new object();
	private static readonly Queue<Action> m_Actions = new Queue<Action>();

	/// <summary>
	/// Gets a value indicating whether or not the current thread is the game's main thread.
	/// </summary>
	public static bool isMainThread
	{
		get
		{
			return Thread.CurrentThread == m_MainThread;
		}
	}

	public static void Init()
	{
		if (m_Instance == null)
		{
			GameObject go = new GameObject();
			go.name = "File Browser Dispatcher";

			m_Instance = go.AddComponent<FileBrowserDispatcher>();
		}
	}

	/// <summary>
	/// Queues an action to be invoked on the main game thread.
	/// </summary>
	/// <param name="action">The action to be queued.</param>
	public static void InvokeAsync(Action action)
	{
		if (m_Instance == null)
		{
			Debug.LogError("You must first call FileBrowserDispatcher.Init() from the Unity main thread");
			return;
		}

		if (isMainThread)
		{
			// Don't bother queuing work on the main thread; just execute it.
			action();
		}
		else
		{
			lock (m_LockObject)
			{
				m_Actions.Enqueue(action);
			}
		}
	}

	void Awake()
	{
		if (m_Instance != null)
		{
			DestroyImmediate(this);
		}
		else
		{
			m_Instance = this;
			m_MainThread = Thread.CurrentThread;
		}
	}

	void OnDestroy()
	{
		if (m_Instance == this)
		{
			m_Instance = null;
		}
	}

	void Update()
	{
		lock (m_LockObject)
		{
			while (m_Actions.Count > 0)
			{
				m_Actions.Dequeue()();
			}
		}
	}
}