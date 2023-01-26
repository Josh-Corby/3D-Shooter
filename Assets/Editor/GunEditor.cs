using UnityEditor;

[CustomEditor(typeof(GunSO))]
public class GunEditor : Editor
{
    #region SerializedProperties
    SerializedProperty bulletToFire;

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
        bulletToFire = serializedObject.FindProperty("bulletToFire");

        shootForce = serializedObject.FindProperty("shootForce");
        timeBetweenShots = serializedObject.FindProperty("timeBetweenShots");
        holdToFire = serializedObject.FindProperty("holdToFire");

        useSpread = serializedObject.FindProperty("useSpread");
        spreadAmount = serializedObject.FindProperty("spreadAmount");

        shotgunFire = serializedObject.FindProperty("shotgunFire");
        shotsInShotgunFire = serializedObject.FindProperty("shotsInShotgunFire");

        burstFire = serializedObject.FindProperty("burstFire");
        bulletsInBurst = serializedObject.FindProperty("bulletsInBurst");
        timeBetweenBurstShots = serializedObject.FindProperty("timeBetweenBurstShots");
    }
    public override void OnInspectorGUI()
    {
        GunSO gun = (GunSO)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(bulletToFire);

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
