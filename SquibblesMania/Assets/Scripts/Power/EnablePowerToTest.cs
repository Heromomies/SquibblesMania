using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePowerToTest : MonoBehaviour
{
    public List<GameObject> powers;
    private bool _activeShield;
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
		    case 3 : _activeShield = true;
			    break;
	    }
	    if (_activeShield)
	    {
		    ShieldPower shield = Instantiate(powers[3].GetComponent<ShieldPower>(), transform.position, Quaternion.identity);
		    if (shield.activated)
		    {
			    shield.ChangeTurn();
		    }
	    }
    }
}
