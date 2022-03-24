using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Light;

public  class CardEffect
{
    public GameObject SetActiveCardEffect(Transform parent, LIGHT_COLOR lightColor)
    {
        GameObject cardEffectGameObject = null;
        
        switch (lightColor)
        {
            case LIGHT_COLOR.COLOR_RED:
                cardEffectGameObject = PoolManager.Instance.SpawnObjectFromPool("HeartCardVFX", new Vector3(0,0,0), Quaternion.identity, parent);
                break;
            case LIGHT_COLOR.COLOR_BLUE:
                cardEffectGameObject = PoolManager.Instance.SpawnObjectFromPool("PikeCardVFX", new Vector3(0,0,0), Quaternion.identity, parent);
                break;
            case LIGHT_COLOR.COLOR_GREEN:
                cardEffectGameObject = PoolManager.Instance.SpawnObjectFromPool("CloverCardVFX", new Vector3(0,0,0), Quaternion.identity, parent);
                break;
            case LIGHT_COLOR.COLOR_YELLOW:
                cardEffectGameObject = PoolManager.Instance.SpawnObjectFromPool("TileCardVFX", new Vector3(0,0,0), Quaternion.identity, parent);
                break;
        }

        SetUpParticleForCanvas(cardEffectGameObject, parent);
        
        ParticleSystem cardEffect = cardEffectGameObject.GetComponent<ParticleSystem>();
        
        ParticleSystem.MainModule cardEffectMain = cardEffect.main;
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = cardEffect.velocityOverLifetime;

        //Convert degree angle into radiant angle
        float rotX = Mathf.Deg2Rad * 180f; 
        
        if (GameManager.Instance.actualCamPreset.presetNumber >= 3)
        {
            cardEffectMain.startRotationXMultiplier = rotX;
            velocityOverLifetime.z = -velocityOverLifetime.z.constant;
        }
        else
        {
            cardEffectMain.startRotationXMultiplier = 0;
            velocityOverLifetime.z = velocityOverLifetime.z.constant;
        }
        
        return cardEffectGameObject;
    }

    void SetUpParticleForCanvas(GameObject particle, Transform parent)
    {
        Vector3 screenPos = UiManager.Instance.uiCam.ScreenToViewportPoint(parent.transform.position);
        particle.transform.localPosition = screenPos;
        particle.transform.localRotation = Quaternion.Euler(-90f, 0f,0f);
        particle.transform.localScale = Vector3.one;
    }
}