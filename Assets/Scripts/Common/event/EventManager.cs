using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
	/*
	private Dictionary <string, ThisEvent> eventDictionary;

	private static EventManagerOneArgument eventManagerArgs;

	public static EventManagerOneArgument instance
	{
		get
		{
			if (!eventManagerArgs)
			{
				eventManagerArgs = FindObjectOfType (typeof (EventManagerOneArgument)) as EventManagerOneArgument;

				if (!eventManagerArgs)
				{
					Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
				}
				else
				{
					eventManagerArgs.Init ();
				}
			}

			return eventManagerArgs;
		}
	}

	void Init ()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, ThisEvent>();
		}
	}

	public static void startListening(string eventName, UnityAction<int> listener){
		ThisEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		}
		else
		{
			thisEvent = new ThisEvent();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventName, thisEvent);
		}
	}

	public static void stopListening(string eventName, UnityAction<int> listener){
		if (eventManagerArgs == null) return;
		ThisEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	public static void triggerEvent(string eventName, int value){
		ThisEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke (value);
		}
	}*/
}
