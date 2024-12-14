using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    private Vector3 pivot;
    private Vector3 pivot1 = new Vector3(2f, 0, 2f);
    private Vector3 pivot2 = new Vector3(3.5f, 0, 2.5f);
    private Vector3 pivot3 = new Vector3(3.5f, 0, 3.5f);

    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        GameObject game1 = GameObject.FindWithTag("game1");
        if (game1) pivot = pivot1;
        GameObject game2 = GameObject.FindWithTag("game2");
        if (game2) pivot = pivot2;
        GameObject game3 = GameObject.FindWithTag("game3");
        if (game3) pivot = pivot3;

    }

    // Update is called once per frame
    void Update()
    {
        offset = gameObject.transform.position - pivot;
        if (Input.GetKey(KeyCode.LeftArrow))
        {

            gameObject.transform.position = pivot + Quaternion.Euler(0, 1f, 0) * offset;
            gameObject.transform.rotation *= Quaternion.Euler(0, 1f, 0);
            
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {

            gameObject.transform.position = pivot + Quaternion.Euler(0, -1f, 0) * offset;
            gameObject.transform.rotation *= Quaternion.Euler(0, -1f, 0);
        }else if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 newPosition = gameObject.transform.position;
            if (newPosition.y < 7)
            {
                newPosition.y += 0.05f;
                gameObject.transform.position = newPosition;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 newPosition = gameObject.transform.position;
            if (newPosition.y > 0)
            {
                newPosition.y -= 0.05f;
                gameObject.transform.position = newPosition;
            }
        }
        gameObject.transform.LookAt(pivot);
    }
}

