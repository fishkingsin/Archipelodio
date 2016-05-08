using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace AssemblyCSharp
{
	[System.Serializable]
	public struct Utils
	{
		static public float Map(float x, float in_min, float in_max, float out_min, float out_max)
		{
			return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
		}
		static public double Mapf(float value, float inputMin, float inputMax, float outputMin, float outputMax, bool clamp) {

//			if (Math.Abs(inputMin - inputMax) < 1.19209290E-07F){
//				return outputMin;
//			} else {
				float outVal = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);

				if( clamp ){
					if(outputMax < outputMin){
						if( outVal < outputMax )outVal = outputMax;
						else if( outVal > outputMin )outVal = outputMin;
					}else{
						if( outVal > outputMax )outVal = outputMax;
						else if( outVal < outputMin )outVal = outputMin;
					}
				}
				return outVal;
//			}

		}

		public static T[] GetAtPath<T> (string path ) {

			ArrayList al = new ArrayList();
			string [] fileEntries = Directory.GetFiles(Application.dataPath+"/"+path);
			foreach(string fileName in fileEntries)
			{
				int index = fileName.LastIndexOf("/");
				string localPath = "Assets/" + path;

				if (index > 0)
					localPath += fileName.Substring(index);

				UnityEngine.Object t = Resources.Load(localPath, typeof(T));

				if(t != null)
					al.Add(t);
			}
			T[] result = new T[al.Count];
			for(int i=0;i<al.Count;i++)
				result[i] = (T)al[i];

			return result;
		}
		private static System.Random rng = new System.Random();  
		public static void Shuffle<T>(ref System.Collections.Generic.List<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
	}
}

