using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {

#pragma warning disable 0649
  [SerializeField] float secondsToWait;
#pragma warning restore 0649

  private void Start() {
    StartCoroutine(SelfDestruct());
  }

  private IEnumerator SelfDestruct() {
    yield return new WaitForSeconds(secondsToWait);
    Destroy(gameObject);
  }
}
