using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dice
{

	public static int RandomDice(int numDice)
    {
        Random.InitState(System.DateTime.Now.Second);
        int accum = 0;
        while (numDice-- > 0)
        {
            accum += Random.Range(0, 3);
        }
        return accum;
    }
}
