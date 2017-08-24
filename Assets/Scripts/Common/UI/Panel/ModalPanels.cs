using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class ModalPanels {
	private static IDictionary<ModalPanelName, ModalPanel> panels;

	public static void Init () {
		if(panels == null) {
			panels = new Dictionary<ModalPanelName, ModalPanel>();
			foreach(UnityEngine.Object obj in UnityEngine.Object.FindObjectsOfTypeAll(typeof(ModalPanel)))  {
				ModalPanel panel = (ModalPanel)obj;
				ModalPanelName name = (ModalPanelName)Enum.Parse(typeof(ModalPanelName), panel.gameObject.name); 
				Preconditions.Check(!panels.ContainsKey(name), "Duplicated ModalPanel with name {0}", name.ToString());
				panels.Add(name, panel);
			}
		}
	}

	public static ModalPanel Show(ModalPanelName name, string text = null, UnityAction yesEvent = null, UnityAction noEvent = null, UnityAction closeEvent = null) {
		if(name == ModalPanelName.MessagePanel) {
			name = ModalPanelName.ErrorPanel;
		}

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
