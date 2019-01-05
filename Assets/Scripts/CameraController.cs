using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour {

  [SerializeField] private Player player;
  [SerializeField] private RectTransform leftPanel;
  [SerializeField] private RectTransform bottomPanel;
  private new Camera camera;

  //For use in Mathf.SmoothDamp calls
  [SerializeField] private float smoothTime = 0.1f;
  private float xVelocity = 0.0f;
  private float yVelocity = 0.0f;
  private float zVelocity = 0.0f;

  private Vector3 offset;

  //For resizing
  private Vector2 resolution;
  private Vector2 originalXy;

  private void Awake() {
    camera = GetComponent<Camera>();
    originalXy = new Vector2(camera.pixelRect.x, camera.pixelRect.y);
    StartCoroutine(OnResizeRoutine());
  }

  private void Start() {
    Vector3 playerPosition = player.transform.position;
    Vector3 cameraPosition = transform.position;
    offset = cameraPosition - playerPosition;
  }

  //CameraController.camera follows CameraController.player
  private void LateUpdate() {
    Vector3 playerPosition = player.transform.position;
    Vector3 cameraPosition = transform.position;

    cameraPosition.x = Mathf.SmoothDamp(cameraPosition.x, playerPosition.x + offset.x, ref xVelocity, smoothTime);
    cameraPosition.y = Mathf.SmoothDamp(cameraPosition.y, playerPosition.y + offset.y, ref yVelocity, smoothTime);
    cameraPosition.z = Mathf.SmoothDamp(cameraPosition.z, playerPosition.z + offset.z, ref zVelocity, smoothTime);

    transform.position = cameraPosition;
  }

  //Adjusts CameraController.camera in case the screen has resized
  private IEnumerator OnResizeRoutine() {
    while (true) {
      //Can't condition on whether resolution == (Screen.width, Screen.height)
      //Starting game in inspector with "maximize on play" enabled will fail

      resolution.x = Screen.width;
      resolution.y = Screen.height;

      Rect cameraRect = camera.pixelRect;

      cameraRect.width = resolution.x;
      cameraRect.height = resolution.y;
      cameraRect.x = originalXy.x + leftPanel.rect.width;
      cameraRect.y = originalXy.y + bottomPanel.rect.height;
      camera.pixelRect = cameraRect;
      yield return new WaitForSeconds(2f);
    }
  }
}
