using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

#pragma warning disable 0649
  [SerializeField] private Sound[] _sounds = (Sound[])System.Enum.GetValues(typeof(Sound));
  [SerializeField] private AudioSource[] _audioClips;
#pragma warning restore 0649
  private Dictionary<Sound, AudioSource> sounds;

  public enum Sound { DAMAGE, DEATH, BGM, FIRE, CUT, WALK }

  private static SoundManager S { get; set; }

  private void Awake() {
    S = this;

    sounds = new Dictionary<Sound, AudioSource>();
    for (int i = 0; i < _sounds.Length; i++) {
      if (sounds.ContainsKey(_sounds[i])) {
        throw new System.Exception("Duplicate keys found");
      }
      sounds.Add(_sounds[i], _audioClips[i]);
    }

    sounds[Sound.BGM].Play();
    sounds[Sound.BGM].Pause();
  }

  public static void ToggleBgm(bool turnOn) {
    if (turnOn) {
      S.sounds[Sound.BGM].UnPause();
    } else {
      S.sounds[Sound.BGM].Pause();
    }
  }

  public static void PlaySound(Sound sound) {
    if (sound == Sound.BGM) throw new System.ArgumentException("Use ToggleBgm instead");
    S.sounds[sound].Play();
  }

  public static IEnumerator PlaySoundAndWait(Sound sound) {
    if (sound == Sound.BGM) throw new System.ArgumentException("Why on earth would you wait for the BGM?  <_<");
    S.sounds[sound].Play();
    yield return new WaitUntil(() => !S.sounds[sound].isPlaying);
  }
}
