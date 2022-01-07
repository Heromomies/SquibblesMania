using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Event
{
   [CreateAssetMenu (fileName = "Scriptable Object", menuName = "Scriptable Object/Conditions Release Event")]
   public class ConditionReleaseEvent : ScriptableObject
   {
      public Condition[] conditions;
   }
}
[System.Serializable]
public class Condition
{
   public enum ConditionType
   {
      NumberOfSteps = 0,
      WalkOnCase = 1, 
      MoveCase = 2
   }
   
   public enum ColorToChose
   {
      Red = 0,
      Yellow = 1, 
      Blue = 2,
      Green = 3
   }
   
   public ConditionType conditionType;
   public ColorToChose colorsToChose;
 
   public int numberOfSteps;
   
   public string conditionToRelease;
}
