using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TurretLevel
{
    public float range,cd;
    public int damage;
}

[Serializable]
public class SlowLevel : TurretLevel
{
    public float slowAmount;
}

[Serializable]

public class ShotgunLevel : TurretLevel
{
    public int pellets;
}


public class CollectionManager : SingletonClass<CollectionManager>
{
    public List<GameObject> bubbles;

    [Header("Turrets Level Data")]
    public List<TurretLevel> aoeturretLevels;
    public List<SlowLevel> slowturretLevels;
    public List<TurretLevel> rapidfireturretLevels;
    public List<ShotgunLevel> shotgunturretLevels;

    [Header("Turrets prefabs")]
    public GameObject aoeturretPrefab;
    public GameObject slowturretPrefab;
    public GameObject rapidfireturretPrefab;
    public GameObject shotgunturretPrefab;

    [Header("Turrets Sprites")]
    public List<Sprite> aoeturretSprites;
    public List<Sprite> slowturretSprites;
    public List<Sprite> rapidfireturretSprites;
    public List<Sprite> shotgunturretSprites;

    [Header("Projectiles")]
    public List<Sprite> generalProjectiles;
    public Sprite slowProjectile;



    public GameObject GetTurretPrefab(TurretType turretType)
    {
        switch (turretType)
        {
            case TurretType.Aoe:
                return aoeturretPrefab;
            case TurretType.Slow:
                return slowturretPrefab;
            case TurretType.RapidFire:
                return rapidfireturretPrefab;
            case TurretType.Shotgun:
                return shotgunturretPrefab;
            default:
                throw new ArgumentOutOfRangeException(nameof(turretType), turretType, null);
        }
    }

}
