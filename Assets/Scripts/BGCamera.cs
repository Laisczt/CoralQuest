using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCamera : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] SpriteRenderer background;
    //public Vector3 MapTopLeftCorner;
    //public Vector3 MapBottomRightCorner;

    private Vector2 mapSize;
    private Vector2 offsets;

    private Vector3 bgBounds;
    private float cameraSize;

    Vector3 newPos;

    public static BGCamera Instance;

    private void Awake()
    {
        cameraSize = GetComponent<Camera>().orthographicSize;
    }
    private void Start()
    {
        Instance = this;
        mainCamera = Camera.main;
        bgBounds = background.sprite.bounds.extents;
        bgBounds.y -= cameraSize;
        newPos.z = transform.localPosition.z;

        var levelCol = GameObject.Find("Level").GetComponent<Collider2D>();
        mapSize = levelCol.bounds.extents;
        offsets = levelCol.offset;
        //mapSize = new Vector2(MapBottomRightCorner.x - MapTopLeftCorner.x, MapTopLeftCorner.y - MapBottomRightCorner.y);
        //offsets = new Vector2((MapBottomRightCorner.x + MapTopLeftCorner.x) /2, (MapTopLeftCorner.y + MapBottomRightCorner.y) / 2);

    }

    // Update is called once per frame
    void Update()
    {
        var tx = (mainCamera.transform.position.x - offsets.x) / (mapSize.x);
        if(tx <= 1 && tx >= -1)
        {
            newPos.x = tx * bgBounds.x;
        }

        var ty = (mainCamera.transform.position.y - offsets.y) / (mapSize.y);
        //Debug.Log(tx);
        if (ty <= 1 && ty >= -1)
        {
            newPos.y = ty * bgBounds.y;
        }

        transform.localPosition = newPos;
    }

    public void ApplyAspectRatio(float aspectRatio)
    {
        bgBounds.x -= aspectRatio * cameraSize;
    }
}
