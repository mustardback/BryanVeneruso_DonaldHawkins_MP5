using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    public GameObject lookAt;

    private bool hasLastLeftMousePos = false;
    private bool hasLastRightMousePos = false;
    private Vector3 lastLeftMousePoint = Vector3.zero;
    private Vector3 lastRightMousePoint = Vector3.zero;
    private Vector2 leftDragAmount = Vector2.zero;
    private Vector2 rightDragAmount = Vector2.zero;

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {

            DollyZoom();
            Track();
            ComputeDrag();

            //Support tumble
            if (leftDragAmount.magnitude > 0)
            {
                //Do vertical tumbling in increments to prevent going over top too fast
                float vertAmount = leftDragAmount.y * -0.2f;
                while (Mathf.Abs(vertAmount) > .0001f)
                {
                    float amount = Mathf.Min(.5f, Mathf.Abs(vertAmount)) * vertAmount / Mathf.Abs(vertAmount);
                    vertAmount -= amount;
                    VerticleTumbleAngleAxis(amount);
                }
                HorizontalTumbleAngleAxis(leftDragAmount.x * 0.2f);
            }

        }

        Vector3 forward = (lookAt.transform.localPosition - transform.localPosition).normalized;
        Vector3 right = Vector3.Cross(forward, Vector3.up);
        Vector3 up = Vector3.Cross(right, forward);
        transform.localRotation = Quaternion.LookRotation(forward, up);
    }

    private void VerticleTumbleAngleAxis(float amount)
    {
        RotateAroundAngleAxis(amount, transform.right);
    }

    private void HorizontalTumbleAngleAxis(float amount)
    {
        RotateAroundAngleAxis(amount, Vector3.up);
    }

    //This is the math provided in example 6 week 6, tumble
    private void RotateAroundAngleAxis(float degrees, Vector3 axis)
    {
        Quaternion q = Quaternion.AngleAxis(degrees, axis);
        Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        Matrix4x4 invP = Matrix4x4.TRS(-lookAt.transform.localPosition, Quaternion.identity, Vector3.one);
        r = invP.inverse * r * invP;
        Vector3 newCameraPos = r.MultiplyPoint(transform.localPosition);

        //If we're in a valid verticaL tumble angle range
        float oldVert = Mathf.Abs(Vector3.Dot((transform.localPosition - lookAt.transform.localPosition).normalized, Vector3.up));
        float newVertAmount = Mathf.Abs(Vector3.Dot((newCameraPos - lookAt.transform.localPosition).normalized, Vector3.up));
        if (newVertAmount < .999f || newVertAmount < oldVert)
            transform.localPosition = newCameraPos;
    }

    private void DollyZoom()
    {
        //Support dolly zooming
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        float moveAmount = scrollAmount * 3;
        if (moveAmount > .0001 || moveAmount < -.0001)
        {
            moveAmount = Mathf.Min(moveAmount, (lookAt.transform.localPosition - transform.localPosition).magnitude - 1);
            transform.localPosition += transform.forward * moveAmount;
        }
    }

    private void Track()
    {
        //Support track
        if (rightDragAmount.magnitude > 0)
        {
            float trackMod = .03f;
            Vector3 posChange = transform.right * rightDragAmount.x * trackMod + transform.up * rightDragAmount.y * trackMod;
            transform.localPosition += posChange;
            lookAt.transform.localPosition += posChange;
        }
    }

    private void ComputeDrag()
    {

        leftDragAmount = Vector2.zero;
        if (Input.GetMouseButton(0))
        {
            if (hasLastLeftMousePos)
            {
                leftDragAmount.x = Input.mousePosition.x - lastLeftMousePoint.x;
                leftDragAmount.y = Input.mousePosition.y - lastLeftMousePoint.y;
            }
            hasLastLeftMousePos = true;
            lastLeftMousePoint = Input.mousePosition;
        }
        else
        {
            hasLastLeftMousePos = false;
        }

        rightDragAmount = Vector2.zero;
        if (Input.GetMouseButton(1))
        {
            if (hasLastRightMousePos)
            {
                rightDragAmount.x = Input.mousePosition.x - lastRightMousePoint.x;
                rightDragAmount.y = Input.mousePosition.y - lastRightMousePoint.y;
            }
            hasLastRightMousePos = true;
            lastRightMousePoint = Input.mousePosition;
        }
        else
        {
            hasLastRightMousePos = false;
        }

    }
}
