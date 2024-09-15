using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCameraController : MonoBehaviour
{
    public Transform target, camTransform;
    [Range(1, 100)]
    public float distance;
    [Range(1, 10)]
    public float sensitivityX, sensitivityY, scrollSensitivity, distancePanAmount;
    [Range(-100, 100)]
    private float currentX, currentY, i, distancePan, ii;
    public bool follow;
    public GameObject player;
    Vector3 a, b;

    // Define min and max vertical angles
    public float minYAngle = -45.0f;
    public float maxYAngle = 45.0f;
    public GraphicRaycaster graphicRaycaster;

    void Start()
    {
        i = 0.5f;
        camTransform = transform;                                                               //  grab transform of self
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        a = new Vector3(target.position.x, target.position.y, target.position.z);
        b = a;
    }

    void Update()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * 10;

        if (distance <= 3)
        {
            distance = 3;
        }
        else if (distance >= 100)
        {
            distance = 100;
        }

        distancePan = /*Input.GetAxis("Vertical") * */ 2.5f * distancePanAmount;
        currentX += Input.GetAxis("Mouse X");   // + (Input.GetAxis("Horizontal 2") * 1);
        currentY += Input.GetAxis("Mouse Y");   // + (Input.GetAxis("Vertical 2") * 1);
        // player.transform.rotation = Quaternion.Euler(0, currentX * sensitivityX + 180f, 0);
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    private void LateUpdate()
    {
        if (follow)
        {
            Vector3 dir = new Vector3(0, 0, distance);
            Vector3 dir2 = new Vector3(0, 0, distance + distancePan);
            dir = Vector3.Lerp(dir, dir2, i);
            Quaternion rotation = Quaternion.Euler(currentY * sensitivityY, currentX * sensitivityX, 0);
            camTransform.position = target.position + rotation * dir;
            camTransform.position += new Vector3(0, 1.5f, 0);
        }


        a = new Vector3(target.position.x, target.position.y + 1.5f, target.position.z);

        ii += 0.01f;

        if (ii > 1.0f) //seconds
        {
            b = new Vector3(target.position.x, target.position.y, target.position.z);
        }
        transform.LookAt(a);

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Screen.width / 2, Screen.height / 2)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        if (graphicRaycaster != null)
            graphicRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ExecuteEvents.Execute(result.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
            }
        }
    }
}
