// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleMain.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Slash.Unity.UI.Windows;
using UnityEngine;

public class LoadWindow : MonoBehaviour {
	#region Fields

	public string WindowId = "Start";

	#endregion

	#region Methods

	protected void Start() {
		// Load main window.
		WindowManager.Instance.OpenWindow(this.WindowId, false);
	}

	#endregion
}
