using UnityEngine;
using UnityEngine.SceneManagement;

public class TextSceneBeforeLevel : MonoBehaviour
{
	public string NextSceneName;

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.Space)) {
			SceneManager.LoadScene(NextSceneName);
		}
	}
}
