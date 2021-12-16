using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "Scriptable Object", menuName = "Scriptable Object/Conditions Release Event")]
public class ConditionReleaseEvent : ScriptableObject
{
   public bool actionPoint;
   public bool walkOnCase;
   public bool walkOnColor;
}
