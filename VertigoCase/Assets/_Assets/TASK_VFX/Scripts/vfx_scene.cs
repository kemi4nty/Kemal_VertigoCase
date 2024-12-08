using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class vfx_scene : MonoBehaviour
{
   
   [SerializeField] private GameObject[] toggleObj;

   public void CheckAnimate()
   {
        var _isToggle = gameObject.GetComponent<Toggle>().isOn;
            
        toggleObj[0].SetActive(_isToggle);
        toggleObj[1].SetActive(!_isToggle);
   }
}


