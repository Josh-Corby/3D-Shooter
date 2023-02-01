using UnityEditor;

[CustomEditor(typeof(BulletSO))]
public class BulletEditor : Editor
{
    #region SerializedProperties

    SerializedProperty hasRigidBody;

    SerializedProperty splittingProjectile;
    SerializedProperty splitBullet;
    SerializedProperty splitForce;
    SerializedProperty splitSpread;

    SerializedProperty homingProjectile;
    SerializedProperty maxHomingDistance;
    SerializedProperty homingSpeed;
    SerializedProperty maxSpeed;
    SerializedProperty findTargetWaitTime;

    SerializedProperty explodingProjectile;
    SerializedProperty explosionRadius;
    SerializedProperty explosionDamage;
    #endregion

    private int UISpace = 7;
    private void OnEnable()
    {
        hasRigidBody = serializedObject.FindProperty(nameof(hasRigidBody));

        splittingProjectile = serializedObject.FindProperty(nameof(splittingProjectile));
        splitBullet = serializedObject.FindProperty(nameof(splitBullet));
        splitForce = serializedObject.FindProperty(nameof(splitForce));
        splitSpread = serializedObject.FindProperty(nameof(splitSpread));

        homingProjectile = serializedObject.FindProperty(nameof(homingProjectile));
        maxHomingDistance = serializedObject.FindProperty(nameof(maxHomingDistance));
        homingSpeed = serializedObject.FindProperty(nameof(homingSpeed));
        maxSpeed = serializedObject.FindProperty(nameof(maxSpeed));
        findTargetWaitTime = serializedObject.FindProperty(nameof(findTargetWaitTime));

        explodingProjectile = serializedObject.FindProperty(nameof(explodingProjectile));
        explosionRadius = serializedObject.FindProperty(nameof(explosionRadius));
        explosionDamage = serializedObject.FindProperty(nameof(explosionDamage));
    }

    public override void OnInspectorGUI()
    {
        BulletSO bullet = (BulletSO)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(hasRigidBody);

        EditorGUILayout.Space(UISpace);

        EditorGUILayout.PropertyField(splittingProjectile);
        if (bullet.splittingProjectile)
        {
            EditorGUILayout.PropertyField(splitBullet);
            EditorGUILayout.PropertyField(splitForce);
            EditorGUILayout.PropertyField(splitSpread);
        }

        EditorGUILayout.Space(UISpace);

        EditorGUILayout.PropertyField(homingProjectile);
        if (bullet.homingProjectile)
        {
            bullet.hasRigidBody = true;
            EditorGUILayout.PropertyField(maxHomingDistance);
            EditorGUILayout.PropertyField(homingSpeed);
            EditorGUILayout.PropertyField(maxSpeed);
            EditorGUILayout.PropertyField(findTargetWaitTime);
        }

        EditorGUILayout.Space(UISpace);

        EditorGUILayout.PropertyField(explodingProjectile);
        if (bullet.explodingProjectile)
        {
            EditorGUILayout.PropertyField(explosionRadius);
            EditorGUILayout.PropertyField(explosionDamage);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
