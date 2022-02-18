using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePowerToTest : MonoBehaviour
{
    public List<GameObject> powers;
   
    public void LaunchPower(int p)
    {
	    switch (p)
	    {
		    case 0 : powers[0].SetActive(true);
			    break;
		    case 1 : powers[1].SetActive(true);
			    break;
		    case 2 : powers[2].SetActive(true);
			    break;
		    case 3 : powers[3].SetActive(true);
			    break;
	    }
    }
}
