using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPanel : MonoBehaviour {

  [SerializeField] private Player player;
  private UnityEngine.UI.Text statText;

  private void Awake() {
    statText = GetComponent<UnityEngine.UI.Text>();
  }

  private void Start() {
    player.ListenStatChanged(OnPlayerStatChanged);
    OnPlayerStatChanged();
  }

  private void OnPlayerStatChanged() {
    statText.text = "" +
      " HP " + player.CurrentHp + "/" + player.MaxHp + "\n" +
      " TP " + player.CurrentTp + "/" + player.MaxTp + "\n" +
      "STR " + player.Str + "\n" +
      "VIT " + player.Vit + "\n" +
      "FOC " + player.Foc + "\n" +
      "SPI " + player.Spi + "\n" +
      "SPD " + player.Spd;
  }
}
