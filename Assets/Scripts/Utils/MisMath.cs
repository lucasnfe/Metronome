using UnityEngine;
using System;
using System.Collections;

public class MisMath {

	public static int[,] Rotate2DArrayClockwise(int[,] mat) {

		int M = mat.GetLength(0);
		int N = mat.GetLength(1);

		int[,] ret = new int[N,M];

		for (int r = 0; r < M; r++) {
			for (int c = 0; c < N; c++) {
				ret[c, M-1-r] = mat[r, c];
			}
		}

		return ret;
	}

	public static float Mean(float []data) {

		float totalData = 0f;
		for (int i = 0; i < data.Length; i++) {
			totalData += data [i];
		}

		return totalData / (float)data.Length;
	}
}
