/* 
 * // Loading UI //
 * Code that causes the arrow to rotate on the screen while loading.
 */

using UnityEngine;
using UnityEngine.UI;

public class rotatetotate : MonoBehaviour {

    private RectTransform rectComponent;
    private Image imageComp;
    private bool up = false;

    public float rotateSpeed = 200f;

    // Use this for initialization
    void Start () 
    {
        //Get arrow images
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();

    }
	
	// Update is called once per frame
	void Update () 
    {
        //rotate setting
        float currentSpeed = rotateSpeed * Time.deltaTime;
        rectComponent.Rotate(0f, 0f, currentSpeed);
    }
}