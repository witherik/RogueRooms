using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWeapon : MonoBehaviour
{
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private float idleRotSpeed = 180f;
    private Quaternion anchorPrevFrameRot;
    void Update()
    {
        if (anchorPrevFrameRot != weaponAnchor.transform.rotation)
        {
            if (Vector2.Dot(Vector2.right, weaponAnchor.right) < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                transform.rotation = weaponAnchor.rotation;
                transform.Rotate(new Vector3(0, 0, 180));

            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                transform.rotation = weaponAnchor.rotation;
            }
        }
        else
        {
            var targetDir = Quaternion.Euler(0, 0, -10);
            if (transform.localScale.x < 0)
            {
                targetDir = Quaternion.Euler(0, 0, 10);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDir, idleRotSpeed * Time.deltaTime);
        }

        anchorPrevFrameRot = weaponAnchor.rotation;

    }
}
