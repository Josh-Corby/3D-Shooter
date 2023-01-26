using UnityEditor;

[CustomEditor(typeof(BulletSO))]
public class BulletEditor : Editor
{
    #region SerializedProperties

    SerializedProperty damage;
    SerializedProperty hasRigidBody;

    SerializedProperty splittingProjectile;
    SerializedProperty splitBullet;
    SerializedProperty splitForce;
    SerializedProperty splitSpread;

    SerializedProperty homingProjectile;
    SerializedProperty homingSpeed;
    SerializedProperty maxSpeed;
    SerializedProperty findTargetWaitTime;
    #endregion

    private void OnEnable()
    {
        damage = serializedObject.FindProperty("damage");
        hasRigidBody = serializedObject.FindProperty("hasRigidBody");

        splittingProjectile = serializedObject.FindProperty("splittingProjectile");
        splitBullet = serializedObject.FindProperty("splitBullet");
        splitForce = serializedObject.FindProperty("splitForce");
        splitSpread = serializedObject.FindProperty("splitSpread");

        homingProjectile = serializedObject.FindProperty("homingProjectile");
        homingSpeed = serializedObject.FindProperty("homingSpeed");
        maxSpeed = serializedObject.FindProperty("maxSpeed");
        findTargetWaitTime = serializedObject.FindProperty("findTargetWaitTime");
    }

    public override void OnInspectorGUI()
    {
        BulletSO bullet = (BulletSO)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(hasRigidBody);

        EditorGUILayout.Space(7);

        EditorGUILayout.PropertyField(splittingProjectile);
        if (bullet.splittingProjectile)
        {
            EditorGUILayout.PropertyField(splitBullet);
            EditorGUILayout.PropertyField(splitForce);
            EditorGUILayout.PropertyField(splitSpread);
        }
        
        EditorGUILayout.Space(7);

        EditorGUILayout.PropertyField(homingProjectile);
        if (bullet.homingProjectile)
        {
            bullet.hasRigidBody = true;
            EditorGUILayout.PropertyField(homingSpeed);
            EditorGUILayout.PropertyField(maxSpeed);
            EditorGUILayout.PropertyField(findTargetWaitTime);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
