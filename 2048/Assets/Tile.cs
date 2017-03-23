using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tile : MonoBehaviour {

    [SerializeField]
    Gradient colors;
    [SerializeField]
    Image img;
    [SerializeField]
    Text txt;

    public int Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public int Position
    {
        get { return _position; }
        set
        {
            _position = value;
        }
    }

    int _value;
    int _position;
	void LateUpdate () 
    {
        img.color = colors.Evaluate(_value / 2048f);
        if(_value != 0)
        {
            txt.text = _value.ToString();
        }
        else
        {
            txt.text = "";
        }
	}
}
