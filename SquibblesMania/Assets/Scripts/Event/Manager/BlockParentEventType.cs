using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParentEventType : MonoBehaviour
{
   public TypeOfBloc typeOfBloc;
   
   public enum TypeOfBloc
   {
      None = 0,
      Powder = 1,
      BreakableIce = 2
   }
}
