using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionPower : MonoBehaviour,IManagePower
{
	public void LaunchPower(int pos)
    {
	    switch (pos)
	    {
		    case 0 : 
			    PowerManager.Instance.targetTouched.position += Vector3.back;
			    break;
		    case 1 : 
			    PowerManager.Instance.targetTouched.position += Vector3.forward;
			    break;
		    case 2 : 
			    PowerManager.Instance.targetTouched.position += Vector3.left;
			    break;
		    case 3 : 
			    PowerManager.Instance.targetTouched.position += Vector3.right;
			    break;
	    }
	    
	    
    }
}
