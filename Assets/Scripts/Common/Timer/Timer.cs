using UnityEngine;


namespace Common.Timer {
	
	public class Timer {
		private float period;
		private float delay;
		private uint? count = null;
		private int curCount;
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
		public Timer SetCount(uint? val) {
			this.count = val;
			if(val != null) {
				curCount = (int)val.Value;
			}
			return this;
		}
		public uint? GetCount() {
			return count;
		}
		public int GetCurCount() {
			return curCount;
		}

		public void Start() {
			if(count != null) {
				curCount = (int)count.Value;
			}
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

			if(count != null) {
				curCount = curCount - res;
				if(curCount < 0) {
					res = res + curCount;
					curCount = 0;
				}

				if(curCount == 0) {
					Stop();
				}
			}

			if(res < 0) {
				res = 0;
			}

			return res;
		}
	}

}
