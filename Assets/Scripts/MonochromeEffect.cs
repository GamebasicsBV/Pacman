using UnityEngine;

public class MonochromeEffect : MonoBehaviour {
	public Material Material;
	public static MonochromeEffect instance;
	private float monochromeCountdownTimer = 0f;

	public static void StartDoingTheMonochrome(float time) {
		if (instance && instance.Material) {
			instance.monochromeCountdownTimer = time;
		}
	}

	// Start is called before the first frame update
	void Start() {
		instance = this;
	}

	// Update is called once per frame
	void Update() {
		if (monochromeCountdownTimer > 0) {
			monochromeCountdownTimer -= Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.P)) {
			ColorDistortEffect.StartDoingTheColorDistort(0.07f);
		}
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (monochromeCountdownTimer > 0) {
			Graphics.Blit(source, destination, Material);
		}
		else {
			// horrible, just disable the component. but hey i'm lazy
			Graphics.Blit(source, destination);
		}
	}
}
