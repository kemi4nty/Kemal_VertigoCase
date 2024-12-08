using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct AttachmentContentProperties
{
   public Image[] categoryImages;
   public Sprite[] categorySprites;
   public GameObject[] selectedIcons;
   public GameObject[] attachmentObjects;
   public int lastSelectedObjectID;
}

[System.Serializable]
public struct StatProperties
{
   public GameObject[] statTitle;
   public TextMeshProUGUI[] statValue;

}

[System.Serializable]
public struct Equips
{
   public int categoryID;
   public int equippedID;
}

public class UIManager : MonoBehaviour
{
   #region GARBAGE

   private float delay_DotFortyFive = .45f;

   #endregion
   
   #region VARIABLES
   
   [Header("Inventory")] 
   [SerializeField] private GameObject inventoryMenu;
   [SerializeField] private Animation inventoryMenuAnimation;
   [Header("Attachment")]
   [SerializeField] private GameObject attachmentsMenu;
   [SerializeField] private Animation attachmentsMenuAnimation;
   [SerializeField] private GameObject[] attachmentCategory_SelectedIcons;
   [SerializeField] private GameObject[] attachmentCategory_Contents;
   [SerializeField] private AttachmentContentProperties[] attachmentContentProperties;
   private int currentAttachmentCategoryID;
   private int currentAttachObjectID;
   [SerializeField] private Animator gunAnimator;
   [Header("Stats")]
   [SerializeField] private Animation statsAnimation;
   [SerializeField] private StatProperties[] statProperties;
   [Header("Equip")] 
   [SerializeField] private TextMeshProUGUI equipText;
   [SerializeField] private Image equipButton;
   public Equips[] equips;
      
   #endregion

   private void Start()
   {
     InitializeEquips();
   }

   #region TRY

   public void Try_OpenMenu()
   {
      inventoryMenuAnimation.Play("Anim_INV_Close");
      Invoke(nameof(Try_OpenMenuDelayed), delay_DotFortyFive);
      
      attachmentsMenu.SetActive(true);
      attachmentsMenuAnimation.Play("Anim_Attachs_Open");
      statsAnimation.Play("Anim_Stats_Inv2Attach");

      AttachCategory_Select(0);   
      AttachDefaultsToGun();
   }
   
   public void OpenAttachmentMenu_Force(int id)
   {
      currentAttachmentCategoryID = id;
      AttachCategory_Select(id);   
      AttachDefaultsToGun();
      
      inventoryMenuAnimation.Play("Anim_INV_Close");
      Invoke(nameof(Try_OpenMenuDelayed), delay_DotFortyFive);
      
      attachmentsMenu.SetActive(true);
      attachmentsMenuAnimation.Play("Anim_Attachs_Open");
      statsAnimation.Play("Anim_Stats_Inv2Attach");

   }

   private void Try_OpenMenuDelayed() => inventoryMenu.SetActive(false);
   
   public void Try_CloseMenu()
   {
      AttachDefaultsToGun();
      inventoryMenu.SetActive(true);
      
      inventoryMenuAnimation.Play("Anim_INV_Open");
      attachmentsMenuAnimation.Play("Anim_Attachs_Close");
      
      statsAnimation.Play("Anim_Stats_Attach2Inv");

      SetGunAnimatorState(5);
   }
   
   private void Try_CloseMenuDelayed()=> attachmentsMenu.SetActive(false);

   #endregion

   #region ATTACHMENTS
   
   public void AttachCategory_Select(int id)
   {
   //    for (var i = 0; i < attachmentCategory_Contents.Length; i++)
   //       AttachObjectShader(i, i);
      
      foreach (var _selectedIcon in attachmentCategory_SelectedIcons)
         _selectedIcon.SetActive(false);
      foreach (var _selectedContent in attachmentCategory_Contents)
         _selectedContent.SetActive(false);
      
      attachmentCategory_SelectedIcons[id].SetActive(true);
      attachmentCategory_Contents[id].SetActive(true);
      
      currentAttachmentCategoryID = id;
      
      SetGunAnimatorState(id);
      CheckEquipState(currentAttachmentCategoryID,attachmentContentProperties[currentAttachmentCategoryID].lastSelectedObjectID);
   }

   public void AttachToGun(int id)
   {
      var _id = 0;
      foreach (var _selectedIcon in attachmentContentProperties[currentAttachmentCategoryID].selectedIcons)
      {
         _selectedIcon.SetActive(_id == id);
         _id++;
      }

      _id = 0;
      
      foreach (var _attachmentObject in attachmentContentProperties[currentAttachmentCategoryID].attachmentObjects)
      {
         _attachmentObject.SetActive(_id == id);
         if (_id == id)
            currentAttachObjectID = _id;
         
         AttachObjectShader(currentAttachmentCategoryID, id);
         attachmentContentProperties[currentAttachmentCategoryID].lastSelectedObjectID = currentAttachObjectID;
         _id++;
      }

      foreach (var _statProperty in statProperties)
      {
         foreach (var _title in _statProperty.statTitle)
         {
            _title.SetActive(false);
         }

         var r = id == 0 ? 0 : Random.Range(0, _statProperty.statTitle.Length);
         _statProperty.statTitle[r].SetActive(true);

         switch (r)
         {
            case 0:

               _statProperty.statValue[0].text = "";
               
               break;

            case 1:

               var rValue2 = Random.Range(123, 987);
               _statProperty.statValue[0].text = "-" + rValue2;
               
               break;
            
            case 2:

               var rValue = Random.Range(123, 987);
               _statProperty.statValue[1].text = "+" + rValue;
               
               break;
         }
         
         
      }
      
      CheckEquipState(currentAttachmentCategoryID, id);
   }

   private void AttachObjectShader(int category, int id)
   {
      var _id = 0;
      foreach (var _attachmentObject in attachmentContentProperties[category].attachmentObjects)
      {
         var _s = _id == id ? 1 : 0;
         _s = id == 0 ? 0 : _s;
         _s = equips[category].equippedID == id ? 0 : _s;
         
         for (var i = 0; i < _attachmentObject.transform.childCount; i++)
         {
            _attachmentObject.transform.GetChild(i).GetComponent<Renderer>().material.SetInt("_Is_Selected", _s);   
         }
         _id++;
      }
   }

   private void AttachDefaultsToGun()
   {
      currentAttachmentCategoryID = 0;

      for (var i = 1; i <= attachmentContentProperties.Length; i++)
      {
         AttachToGun(equips[currentAttachmentCategoryID].equippedID);
         currentAttachmentCategoryID = i;
      }

      currentAttachmentCategoryID = 0;
   }

   private void SetGunAnimatorState(int id)
   {
      gunAnimator.SetInteger("StateInt",id);
   }

   private void InitializeEquips()
   {
      equips = new Equips[5];
      for (var i = 0; i < 5; i++)
      {
         equips[i].categoryID = i;
         equips[i].equippedID = 0;
      }
      
      AttachmentCategoryImageUpdate();
   }

   private void AttachmentCategoryImageUpdate()
   {
      for (var i = 0; i < attachmentContentProperties.Length; i++)
      {
         foreach (var item in attachmentContentProperties[i].categoryImages)
         {
             item.sprite=attachmentContentProperties[i].categorySprites[equips[i].equippedID];
         }
            
      }
   }

   public void Equip()
   {
      equips[currentAttachmentCategoryID].equippedID = currentAttachObjectID;
      CheckEquipState(currentAttachmentCategoryID, currentAttachObjectID);

      AttachmentCategoryImageUpdate();
   }
   
   public void CheckEquipState(int category, int id)
   {
      equipText.text = equips[category].equippedID == id ? "EQUIPPED" : "EQUIP";
      equipButton.color = equipText.text == "EQUIPPED" ? new Color(0.169f, 0.243f, 0.341f) : new Color(0.184f, 0.443f, 0.780f);
      AttachObjectShader(category, id);
   }
   
   
   #endregion
   
}