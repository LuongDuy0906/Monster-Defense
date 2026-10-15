using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked;
    public static bool towerPanelOpened { get; set; } = false;

    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private Sprite towerPlaceholderSprite;

    private SpriteRenderer _spriteRender;
    private Sprite _originalSprite;


    void Awake()
    {
        _spriteRender = GetComponent<SpriteRenderer>();    
        _originalSprite = _spriteRender.sprite;   
    }

    void Update()
    {
        if(towerPanelOpened || Time.timeScale == 0f || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(transform.childCount == 0)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if(hit.collider != null && hit.collider.gameObject == gameObject)
            {
                _spriteRender.sprite = towerPlaceholderSprite;
            } else
            {
                _spriteRender.sprite = _originalSprite;
            }
        }

        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchPressed = false;

        Vector2 screenPos = Vector2.zero;

        if(mousePressed)
        {
            screenPos = Mouse.current.position.ReadValue();
        } else if (Touchscreen.current != null) 
        {
            foreach(var touch in Touchscreen.current.touches)
            {
                if(touch.press.wasPressedThisFrame)
                {
                    touchPressed = true;
                    screenPos = touch.position.ReadValue();
                    break;
                }
            }
        }

        if(mousePressed || touchPressed)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if(raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if(platform != null)
                {
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
    }

    public void PlaceTower(TowerData data)
    {
        Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
        _spriteRender.sprite = null;
    }
}
