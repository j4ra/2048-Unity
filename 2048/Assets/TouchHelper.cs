using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHelper : MonoBehaviour {

	public enum Direction {left, up, right, down, none};

    public static Direction getTouchInput()
    {
        return _dir;
    }

    static Direction _dir = Direction.none;
    Vector2 startPosT = Vector2.zero;
    Vector2 endPosT = Vector2.zero;
    Vector2 startPosM = Vector2.zero;
    Vector2 endPosM = Vector2.zero;
	void Update () 
    {
        _dir = Direction.none;
        handleTouch();
        handleMouse();
	}

    void handleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                startPosT = t.position;
                endPosT = startPosT;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                endPosT = t.position;
            }
            else if (t.phase == TouchPhase.Ended)
            {
                Vector2 dPos = startPosT - endPosT;
                if (dPos.sqrMagnitude > 80)
                {
                    if (Mathf.Abs(dPos.x) > Mathf.Abs(dPos.y))
                    {
                        if (dPos.x < 0)
                        {
                            _dir = Direction.right;
                        }
                        else
                        {
                            _dir = Direction.left;
                        }
                    }
                    else
                    {
                        if (dPos.y < 0)
                        {
                            _dir = Direction.up;
                        }
                        else
                        {
                            _dir = Direction.down;
                        }
                    }
                }
            }
        }
    }


    void handleMouse()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPosM = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            endPosM = Input.mousePosition;
            Vector2 dPos = startPosM - endPosM;
            if (dPos.sqrMagnitude > 80)
            {
                if (Mathf.Abs(dPos.x) > Mathf.Abs(dPos.y))
                {
                    if (dPos.x < 0)
                    {
                        _dir = Direction.right;
                    }
                    else
                    {
                        _dir = Direction.left;
                    }
                }
                else
                {
                    if (dPos.y < 0)
                    {
                        _dir = Direction.up;
                    }
                    else
                    {
                        _dir = Direction.down;
                    }
                }
            }
        }
    }
}
