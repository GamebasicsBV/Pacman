using UnityEngine;

public class ColorDistortEffect : MonoBehaviour
{
	public Material Material;
	private static ColorDistortEffect instance;
	private float distortCountdownTimer = 0f;

	public static void StartDoingTheColorDistort(float time) {
		if (instance && instance.Material) {
			instance.distortCountdownTimer = time;
		}
	}

	// Start is called before the first frame update
	void Start() {
		instance = this;
		Material.SetVector("_XOffsets", new Vector4(-1, 0, 1, 0));
		Material.SetVector("_YOffsets", new Vector4(1, 0, -1, 0));
	}

    // Update is called once per frame
    void Update() {
		if (distortCountdownTimer > 0) {
			Material.SetFloat("_OffsetMultiplier", 0.02f);
			distortCountdownTimer -= Time.deltaTime;
		}
		else {
			Material.SetFloat("_OffsetMultiplier", 0f);
		}

		if (Input.GetKeyDown(KeyCode.P)) {
			ColorDistortEffect.StartDoingTheColorDistort(0.07f);
		}
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (distortCountdownTimer > 0) {
			Graphics.Blit(source, destination, Material);
		}
		else {
			// horrible, just disable the component. but hey i'm lazy
			Graphics.Blit(source, destination);
		}
	}
}
