using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class FloatingOnScreenStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public bool hideOnStart = true;
    // private bool interacted = false;
    public Vector2 Input { get; private set; }
    private void Awake()
    {
        if (hideOnStart)
            HideJoystick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        GameManager.Ins.isPlaying = true;
        MainUI.Ins.SetActiveTut(false);

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
        m_JoystickTransform.anchoredPosition = m_PointerDownPos;
        m_CircleLimit.anchoredPosition = m_PointerDownPos;

        // interacted = true;
        ShowJoystick();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // interacted = false;
        HideJoystick();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out m_DragPos);
        var delta = m_DragPos - m_PointerDownPos;

        delta = Vector2.ClampMagnitude(delta, movementRange);
        m_JoystickTransform.anchoredPosition = m_PointerDownPos + delta;

        Input = new Vector2(delta.x / movementRange, delta.y / movementRange);
        // interacted = false;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_JoystickTransform.anchoredPosition = m_StartPos;
        Input = Vector2.zero;
        // if (!interacted)
        HideJoystick();
    }

    private void OnDisable()
    {
        m_JoystickTransform.anchoredPosition = m_StartPos;
        Input = Vector2.zero;
        HideJoystick();
    }

    private void Start()
    {
        m_StartPos = ((RectTransform)transform).anchoredPosition;
    }

    private void ShowJoystick()
    {
        m_JoystickTransform.gameObject.SetActive(true);
        m_CircleLimit.gameObject.SetActive(true);
    }

    private void HideJoystick()
    {
        m_JoystickTransform.gameObject.SetActive(false);
        m_CircleLimit.gameObject.SetActive(false);
    }

    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    [SerializeField]
    private float m_MovementRange = 120;

    [SerializeField]
    private RectTransform m_JoystickTransform;

    [SerializeField]
    private RectTransform m_CircleLimit;

    private Vector2 m_StartPos;
    private Vector2 m_PointerDownPos;
    private Vector2 m_DragPos;

}