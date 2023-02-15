/*
 * // UI Management Code //
 * Code to manage the overall UI of the Coyote map scene.
*/

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject listUI; //The button to open the list and the list group UI children below it
    public GameObject listBox; //Item UI to be added to list UI
    public Transform contents; //UI Base Showing Item Lists(ListView -> Contenet panel)
    public Camera mainCamera;

    //Called when the listUI button is pressed. (Button On Click)
    public void OpenList()
    {
        if(listUI != null)
        {
            //Register an animation that appears and disappears in the list view
            Animator animator = listUI.GetComponent<Animator>();
            if(animator != null)
            {
                bool isOpen = animator.GetBool("open");
                animator.SetBool("open", !isOpen);
            }

            //If any UI List item exists, clear the item
            var listItem = GameObject.Find("CoyoteBox1");
            if (listItem != null)
            {
                int childs = contents.childCount;
                for (int i = childs - 1; i >= 0; i--)
                    GameObject.Destroy(contents.GetChild(i).gameObject);
            }

            //Add items to the list view as many as the number of detected coyotes in the list so that can be shown
            for (int i = 1; i <= SingletonLatLng.instance.CoyoteLat.Count; i++)
            {
                GameObject listBoxButton = Instantiate(listBox, contents);
                listBoxButton.name = "CoyoteBox" + i;

                //Register a function to run when a button registered as a component is pressed for each item
                Button btn = listBoxButton.GetComponent<Button>();
                string name = "CoyotePin" + (i - 1);
                btn.onClick.AddListener(() => MoveToCoyote(name)); //Pass the name of the mapped coyote pin that matches the item

                //Change the text to be displayed in the item
                Text text = listBoxButton.transform.GetChild(0).GetComponent<Text>();
                text.text = "Coyote" + i;
            }
        }

        //Move the camera from the list to the coyote pin that the user wants to see.
        void MoveToCoyote(string name)
        {
            GameObject coyotePin = GameObject.Find(name);
            mainCamera.transform.position = new Vector3(coyotePin.transform.position.x, 30, coyotePin.transform.position.z);
        }
    }
}
