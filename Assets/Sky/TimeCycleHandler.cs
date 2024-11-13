using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCycleHandler : MonoBehaviour
{
    [SerializeField] ulong _dayLenght = 15000;
    [SerializeField] Material _skyBoxMat;
    [SerializeField] Light _light;

    public Dictionary<string, Color> colors = new Dictionary<string, Color>()
    {
        { "Day", new Color(3, 219, 252) },
        { "Transition", new Color(202,58,0) },
        { "Night", new Color(0, 33, 38) },
    };
    void FixedUpdate()
    {
        ulong timeInCurrenDay = (ulong)(Time.time * 1000) % _dayLenght;
        float quotient = timeInCurrenDay / (float)_dayLenght;
        //print("qutient : "+ quotient);
        //Color colValue;
        //if (quotient < 0.5f)
        //{
        //    colValue = colors["Transition"] * (.5f - quotient) + colors["Day"] * (.5f+quotient);
        //}
        //else
        //{
        //    colValue = colors["Transition"] * (1 - quotient) + colors["Transition"] * quotient;
        //}
        //print("color : " + colValue);
        //_skyBoxMat.SetColor("_SkyTint", colValue);
        //RenderSettings.skybox.SetColor("_SkyTint", colValue);
        _light.transform.rotation = Quaternion.Euler(new Vector3(quotient * 360, 0, 0));
    }
}
