using UnityEngine;

public class DizzyEffect : MonoBehaviour {
	public Material Material;
	public float Duration = 0.6f;
	public float Heaviness = 0.2f;
	private bool dizzinessGoing = false;
	private float dizzinnessTimer = 0f;
	private static DizzyEffect instance;

	public static void StartDoingTheDizzy() {
		if (instance && instance.Material) {
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
			Material.SetFloat("_OffsetMultiplier", Heaviness * Mathf.Sin(Mathf.PI * dizzinnessTimer / Duration));
			if (dizzinnessTimer > Duration) {
				dizzinessGoing = false;
				dizzinnessTimer = 0f;
			}
		}

		if (Input.GetKeyDown(KeyCode.O)) {
			DizzyEffect.StartDoingTheDizzy();
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
