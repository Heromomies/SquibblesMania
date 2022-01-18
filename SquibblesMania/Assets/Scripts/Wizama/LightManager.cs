using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Light;

public class LightManager : MonoBehaviour
{
	#region Singleton

	private static LightManager lightManager;

	public static LightManager Instance => lightManager;
	// Start is called before the first frame update

	private void Awake()
	{
		lightManager = this;
	}

	#endregion

	public LIGHT_INDEX[] lightIndex;
	public LIGHT_COLOR[] lightColors;
	public bool keepOthersColorized;

	public void OnClick()
	{
		LightController.Colorize(lightIndex, lightColors, keepOthersColorized);
	}
}
