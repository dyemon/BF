// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenWindow.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using UnityEngine;
using Slash.Unity.UI.Windows;

public class OpenWindow : MonoBehaviour
{
    #region Fields

    public string WindowId;

    #endregion

    #region Public Methods and Operators

	public void Execute(bool closeAllWindows)
    {
        if (WindowManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(this.WindowId))
            {
				WindowManager.Instance.OpenWindow(this.WindowId, closeAllWindows);
            }
            else
            {
                Debug.LogWarning("No window id set.", this);
            }
        }
        else
        {
            Debug.LogWarning("No window manager found.", this);
        }
    }

    #endregion
}
