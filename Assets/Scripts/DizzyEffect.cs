using UnityEngine;

public class DizzyEffect : MonoBehaviour {
	public Material Material;
	public float Duration;
	public float Heaviness;
	private bool dizzinessGoing;
	private float dizzinnessTimer = 0f;
	private static DizzyEffect instance;

	public static void StartDoingTheDizzy() {
		if (instance) {
			instance.dizzinessGoing = true;
			instance.dizzinnessTimer = 0f;
		}
	}

	private void Start() {
		instance = this;
	}

	void Update() {
		if (dizzinessGoing) {
			dizzinnessTimer += Time.deltaTime;
			Material.SetFloat("_OffsetMultiplier", Heaviness * Mathf.Sin(2 * Mathf.PI * dizzinnessTimer / Duration));
			if (dizzinnessTimer > Duration) {
				dizzinessGoing = false;
				dizzinnessTimer = 0f;
			}
		}
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (dizzinessGoing) {
			Graphics.Blit(source, destination, Material);
		}
		else {
			// horrible, just disable the component. but hey i'm lazy
			Graphics.Blit(source, destination);
		}
	}
}
