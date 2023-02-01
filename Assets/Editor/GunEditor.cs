using UnityEditor;

[CustomEditor(typeof(GunSO))]
public class GunEditor : Editor
{
    #region SerializedProperties
    SerializedProperty bulletToFire;
    SerializedProperty damage;

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
        bulletToFire = serializedObject.FindProperty(nameof(bulletToFire));
        damage = serializedObject.FindProperty(nameof(damage));

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

        EditorGUILayout.PropertyField(bulletToFire);
        EditorGUILayout.PropertyField(damage);

        EditorGUILayout.PropertyField(shootForce);
        EditorGUILayout.PropertyField(timeBetweenShots);
        EditorGUILayout.PropertyField(holdToFire);

        EditorGUILayout.Space(7);

        EditorGUILayout.PropertyField(burstFire);
        if (gun.burstFire)
        {
            EditorGUILayout.PropertyField(bulletsInBurst);
            EditorGUILayout.PropertyField(timeBetweenBurstShots);
        }

        EditorGUILayout.Space(7);

        EditorGUILayout.PropertyField(shotgunFire);
        if (gun.shotgunFire)
        {
            gun.useSpread = true;
            EditorGUILayout.PropertyField(shotsInShotgunFire);
        }

        EditorGUILayout.Space(7);

        EditorGUILayout.PropertyField(useSpread);
        if (gun.useSpread)
        {
            EditorGUILayout.PropertyField(spreadAmount);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
