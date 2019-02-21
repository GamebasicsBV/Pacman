using UnityEngine;

public class MoveInvertEffect : MonoBehaviour {
	public Material Material;
	public float Heaviness = 0.2f;
	private bool effectGoing = false;
	private static MoveInvertEffect instance;
	public Vector2 scrollDirection = new Vector2(0.01f, 0.02f);
	float timePassedX;
	float timePassedY;

	public static void StartDoingTheEffect() {
		if (instance && instance.Material) {
			instance.effectGoing = true;
		}
	}

	public static void StopDoingTheEffect() {
		if (instance && instance.Material) {
			instance.effectGoing = false;
		}
	}

	private void Start() {
		instance = this;
	}

	void Update() {
		timePassedX += Time.deltaTime;
		timePassedY += Time.deltaTime;
		float cycleTimeX = 1 / scrollDirection.x;
		if (timePassedX >= cycleTimeX) {
			timePassedX -= cycleTimeX;
		}
		float cycleTimeY = 1 / scrollDirection.y;
		if (timePassedY >= cycleTimeY) {
			timePassedY -= cycleTimeY;
		}
		Material.SetColor("_EffectOffset", new Color(scrollDirection.x * timePassedX, scrollDirection.y * timePassedY, 0, 0));
		if (effectGoing) {
			Material.SetFloat("_ColorMultiplier", Heaviness);
		}
		else {
			Material.SetFloat("_ColorMultiplier", 0f);
		}

		if (Input.GetKeyDown(KeyCode.I)) {
			StartDoingTheEffect();
		}
		if (Input.GetKeyUp(KeyCode.I)) {
			StopDoingTheEffect();
		}
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (effectGoing) {
			Graphics.Blit(source, destination, Material);
		}
		else {
			// horrible, just disable the component. but hey i'm lazy
			Graphics.Blit(source, destination);
		}
	}
}
