using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chatbox : MonoBehaviour {

#pragma warning disable 0649
  [SerializeField] private Transform content;
  [SerializeField] private UnityEngine.UI.Text messagePrefab;
  [SerializeField] private int maxMessageHistory = 100;
#pragma warning restore 0649

  private Queue<UnityEngine.UI.Text> messages = new Queue<UnityEngine.UI.Text>();

  public void AddMessage(string message) {
    var messageObject = Instantiate(messagePrefab);
    messageObject.text = message;
    messageObject.transform.SetParent(content, false);

    messages.Enqueue(messageObject);
    if (messages.Count > maxMessageHistory) {
      Destroy(messages.Dequeue().gameObject);
    }
  }
}
