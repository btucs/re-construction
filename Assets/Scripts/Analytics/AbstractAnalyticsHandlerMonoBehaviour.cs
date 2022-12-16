using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractAnalyticsHandlerMonoBehaviour : MonoBehaviour
{
  abstract protected void InitializeEvent();
  abstract public void EndEvent();
}
