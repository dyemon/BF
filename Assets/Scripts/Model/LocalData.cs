using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalData {
	public bool SoundOn = true;
	public bool MusicOn = true;
	public int LastLevel = 1;
	public FBUser FBUser;
	public long SrvTime = 0;
}
