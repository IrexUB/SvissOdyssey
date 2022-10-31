using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Slider slider;
    private float waitTimeMAX;
    private float waitTime;

	public void Setup(float time)
	{
        // Assignation des variables
        waitTimeMAX = time;
        slider = GetComponent<Slider>();

        Destroy(gameObject.transform.parent.gameObject, time);
	}

    private void Update()
    {
        waitTime += Time.deltaTime;
        slider.value = waitTime / waitTimeMAX;
    }
}
