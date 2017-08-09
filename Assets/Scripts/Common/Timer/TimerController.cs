using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common.Timer {

	public class TimerController : MonoBehaviour {
		public static TimerController Instance;

		public delegate void OnTimer(string code, Timer timer);
		public event OnTimer onTimer;

		private IDictionary<string, Timer> timers = new Dictionary<string, Timer>();

		void Awake() {
			Instance = this;
		}

		public Timer AddTimer(string code) {
			Timer timer;
			timers.TryGetValue(code, out timer);
			if(timer != null) {
				return timer;
			}

			timers[code] = new Timer();
			return timers[code];
		}

		public bool RemoveTimer(string code) {
			return timers.Remove(code);
		}

		public Timer GetTimer(string code) {
			Timer timer;
			timers.TryGetValue(code, out timer);
			return timer;
		}

		void Update () {
			foreach(KeyValuePair<string, Timer> pair in timers) {
				int fire = pair.Value.Update(Time.deltaTime);
				while(fire-- > 0) {
					if(onTimer != null) {
						onTimer(pair.Key, pair.Value);
					}
				}

			}
		}

		public void StopAll() {
			timers.Clear();
		}			
	}



}