using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerEvent
{
	public static void ShowPanel(CharacterPawn pawn, int panelID)
	{
        Debug.LogFormat("Show panel {0}", panelID);
	}

}
