using UnityEditor;

[CustomEditor(typeof(GunSO))]
public class GunEditor : Editor
{
    #region SerializedProperties
    SerializedProperty user;
    SerializedProperty gunName;
    SerializedProperty bulletToFire;
    SerializedProperty damage;
    SerializedProperty swapInTime;

    SerializedProperty maxAmmo;
    SerializedProperty clipSize;
    SerializedProperty reloadTime;

    SerializedProperty shootForce;
    SerializedProperty timeBetweenShots;
    SerializedProperty holdToFire;

    SerializedProperty useSpread;
    SerializedProperty spreadAmount;

    SerializedProperty shotgunFire;
    SerializedProperty shotsInShotgunFire;

    SerializedProperty burstFire;
    SerializedProperty bulletsInBurst;
    SerializedProperty timeBetweenBurstShots;
    #endregion

    private void OnEnable()
    {
        user = serializedObject.FindProperty(nameof(user));
        gunName = serializedObject.FindProperty(nameof(gunName));
        bulletToFire = serializedObject.FindProperty(nameof(bulletToFire));
        damage = serializedObject.FindProperty(nameof(damage));
        swapInTime = serializedObject.FindProperty(nameof(swapInTime));

        maxAmmo = serializedObject.FindProperty(nameof(maxAmmo));
        clipSize = serializedObject.FindProperty(nameof(clipSize));
        reloadTime = serializedObject.FindProperty(nameof(reloadTime));

        shootForce = serializedObject.FindProperty(nameof(shootForce));
        timeBetweenShots = serializedObject.FindProperty(nameof(timeBetweenShots));
        holdToFire = serializedObject.FindProperty(nameof(holdToFire));

        useSpread = serializedObject.FindProperty(nameof(useSpread));
        spreadAmount = serializedObject.FindProperty(nameof(spreadAmount));

        shotgunFire = serializedObject.FindProperty(nameof(shotgunFire));
        shotsInShotgunFire = serializedObject.FindProperty(nameof(shotsInShotgunFire));

        burstFire = serializedObject.FindProperty(nameof(burstFire));
        bulletsInBurst = serializedObject.FindProperty(nameof(bulletsInBurst));
        timeBetweenBurstShots = serializedObject.FindProperty(nameof(timeBetweenBurstShots));
    }
    public override void OnInspectorGUI()
    {
        GunSO gun = (GunSO)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(user);
        EditorGUILayout.PropertyField(gunName);
        EditorGUILayout.PropertyField(bulletToFire);
        EditorGUILayout.PropertyField(damage);


        switch (gun.user)
        {
            case User.Player:
                EditorGUILayout.PropertyField(maxAmmo);
                EditorGUILayout.PropertyField(clipSize);
                EditorGUILayout.PropertyField(reloadTime);
                EditorGUILayout.PropertyField(swapInTime);
                break;
        }

        EditorGUILayout.PropertyField(shootForce);
        EditorGUILayout.PropertyField(timeBetweenShots);
        switch (gun.user)
        {
            case User.Player:
                EditorGUILayout.PropertyField(holdToFire);
                break;
        }

        EditorGUILayout.PropertyField(burstFire);
        if (gun.burstFire)
        {
            EditorGUILayout.PropertyField(bulletsInBurst);
            EditorGUILayout.PropertyField(timeBetweenBurstShots);
        }


        EditorGUILayout.PropertyField(shotgunFire);
        if (gun.shotgunFire)
        {
            gun.useSpread = true;
            EditorGUILayout.PropertyField(shotsInShotgunFire);
        }


        EditorGUILayout.PropertyField(useSpread);
        if (gun.useSpread)
        {
            EditorGUILayout.PropertyField(spreadAmount);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
