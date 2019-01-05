using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {

  [SerializeField] float secondsToWait;

  private void Start() {
    StartCoroutine(SelfDestruct());
  }

  private IEnumerator SelfDestruct() {
    yield return new WaitForSeconds(secondsToWait);
    Destroy(gameObject);
  }
}
