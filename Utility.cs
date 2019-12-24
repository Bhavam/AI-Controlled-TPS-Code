using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

	public static T[] ShuffleArray<T>(T[] array, int seed) { // T is a generic type 
		System.Random prng = new System.Random (seed); // creates a random seed object

		for (int i =0; i < array.Length -1; i ++)
       {
			int randomIndex = prng.Next(i,array.Length); // Fisher-Yates random assignment  
			T tempItem = array[randomIndex];
			array[randomIndex] = array[i];
			array[i] = tempItem;
		}

		return array;
	}

}