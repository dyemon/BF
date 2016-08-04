using UnityEngine;
using System.Collections;

public class BarrierData {
	public int X1 { get; set;}
	public int Y1 { get; set;}
	public int X2 { get; set;}
	public int Y2 { get; set;}
	public BarrierType Type { get; set; }

	public BarrierData(int x1, int y1, int x2, int y2, BarrierType type) {
		X1 = x1;
		Y1 = y1;
		X2 = x2;
		Y2 = y2;
		this.Type = type;
		Verify();
	}

	public override bool Equals(object obj) {
		if(obj == null || obj.GetType() != typeof(BarrierData)) {
			return false;
		}

		BarrierData bd = (BarrierData)obj;

		return X1 == bd.X1 && Y1 == bd.Y1 && X2 == bd.X2 && Y2 == bd.Y2
		|| X1 == bd.X2 && Y1 == bd.Y2 && X2 == bd.X1 && Y2 == bd.Y1;
	}

	public override int GetHashCode() {
		return (X1 + Y1 + X2 + Y2).GetHashCode();
	}

	public override string ToString() {
		return string.Format("[BarrierData: X1={0}, Y1={1}, X2={2}, Y2={3}, Type={4}]", X1, Y1, X2, Y2, Type);
	}

	private void Verify() {
		if(Vector2.Distance(new Vector2(X1, Y1), new Vector2(X2, Y2)) != 1) {
			throw new System.Exception("Invalid data for barrier " + this);
		}
	}

	public bool Isvertical {
		get { return Y1 == Y2; }
	}
}
