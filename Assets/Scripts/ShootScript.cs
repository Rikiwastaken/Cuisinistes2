using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{

    public InputAction shootAction;

    [Serializable]
    public class GunClass
    {
        public GameObject GunModel;
        public Vector3 GunPosition;
        public Vector3 GunRotation;
        public GameObject Bulletprefab;
        public float damage;
        public float bulletspeed;
        public float GunCD;
    }

    public Transform MainCamera;


    [Header("GunVariables")]
    public List<GunClass> GunList;
    private int currentgun;
    private GameObject currentGunGO;
    private int GunCoolDown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
        ChangeGun(0);
    }

    private void ChangeGun(int newGunID)
    {
        currentgun = newGunID;

        GunClass activegunclass = GunList[currentgun];
        if (currentGunGO != null)
        {
            DestroyImmediate(currentGunGO);
        }
        currentGunGO = Instantiate(activegunclass.GunModel);
        currentGunGO.transform.parent = MainCamera;
        currentGunGO.transform.localPosition = activegunclass.GunPosition;
        currentGunGO.transform.localRotation = Quaternion.Euler(activegunclass.GunRotation);
        SetLayerAllChildren(currentGunGO.transform, LayerMask.NameToLayer("Weapons"));

    }

    // Update is called once per frame
    void Update()
    {
        if (GunCoolDown > 0)
        {
            GunCoolDown--;
        }
        if (shootAction.ReadValue<float>() != 0)
        {
            if (GunCoolDown == 0)
            {
                GunCoolDown = (int)(GunList[currentgun].GunCD / Time.deltaTime);
                currentGunGO.GetComponentInChildren<Animation>().Play();
            }
        }
    }

    void SetLayerAllChildren(Transform root, int layer)
    {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            //            Debug.Log(child.name);
            child.gameObject.layer = layer;
        }
    }

}
