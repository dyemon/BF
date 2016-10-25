﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class ModalPanels : MonoBehaviour {
	private static IDictionary<ModalPanelName, ModalPanel> panels;

	// Use this for initialization
	void Start () {
		init();
	}
	
	private static void init () {
		if(panels == null) {
			panels = new Dictionary<ModalPanelName, ModalPanel>();
			foreach(UnityEngine.Object obj in FindObjectsOfType(typeof(ModalPanel)))  {
				ModalPanel panel = (ModalPanel)obj;
				ModalPanelName name = (ModalPanelName)Enum.Parse(typeof(ModalPanelName), panel.gameObject.name); 
				Preconditions.Check(!panels.ContainsKey(name), "Duplicated ModalPanel with name {0}", name.ToString());
				panels.Add(name, panel);
			}
		}
	}

	public static ModalPanel Show(ModalPanelName name, string text, UnityAction yesEvent = null, UnityAction noEvent = null, UnityAction closeEvent = null) {
		Preconditions.Check(panels.ContainsKey(name), "Can not find panel {0}", name.ToString());
		ModalPanel panel = panels[name];
		panel.Show(text, yesEvent, noEvent, closeEvent);

		return panel;
	}
		
	public static void ClosePanel(ModalPanelName name) {
		Preconditions.Check(panels.ContainsKey(name), "Can not find panel {0}", name.ToString());
		ModalPanel panel = panels[name];
		panel.ClosePanel();
	}
}
