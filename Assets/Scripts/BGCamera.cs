using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCamera : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] SpriteRenderer background;
    public Vector2 MapSize;

    private Vector3 bgBounds;

    Vector3 newPos;

    private void Start()
    {
        mainCamera = Camera.main;
        bgBounds = background.sprite.bounds.extents;
        newPos.z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        newPos.x = (mainCamera.transform.position.x / MapSize.x) * bgBounds.x;
        newPos.y = (mainCamera.transform.position.y / MapSize.y) * bgBounds.y;

        transform.position = newPos;
    }
}
