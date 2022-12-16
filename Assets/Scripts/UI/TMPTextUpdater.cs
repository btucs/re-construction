using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPTextUpdater : MonoBehaviour
{
	public TextMeshProUGUI textElement;

	public void SetText(string newText)
	{
		textElement.text = newText;
	}
}
