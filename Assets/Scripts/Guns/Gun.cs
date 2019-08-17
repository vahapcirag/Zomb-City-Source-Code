using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun",menuName ="Gun",order =1)]
public class Gun : ScriptableObject
{
    public string gunName;

    public Transform gunRefTransform;

    public Sprite gunBulletSprite;

    public short gunDamage;
    public short gunBulletCapacity;
    public short gunMaxBullet;
    public short gunReloadTime;

    public float gunTimeBetweenBullets;
    public float bulletSpeed;

    public bool gunIsFullAutomatic;
    public bool gunIsReloading;

}

