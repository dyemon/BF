using UnityEngine;


namespace Common.Timer {
	
	public class Timer {
		private float period;
		private float delay;
		private float curVal;
		private bool start = false;

		public Timer SetPeriod(float period) {
			this.period = period;
			this.curVal = period;
			return this;
		}
		public float GetPeriod() {
			return period;
		}

		public Timer SetDelay(float delay) {
			this.delay = delay;
			return this;
		}
		public float GetDelay() {
			return delay;
		}

		public void Start() {
			start = true;
		}
		public void Stop() {
			start = false;
		}

		public int Update(float delta) {
			if(!start) {
				return 0;
			}

			if(delay > 0) {
				delay -= delta;
				if(delay >= 0) {
					return 0;
				}
				delta = -delay;
			}

			curVal -= delta;
			if(curVal > 0) {
				return 0;
			}
			if(curVal == 0) {
				curVal = period;
				return 1;
			}

			int res = (int)Mathf.Ceil((-curVal) / period);
			curVal = period - (-curVal - (res - 1) * period);

			return res;
		}
	}

}
