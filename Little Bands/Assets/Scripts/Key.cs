using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Key : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    public AudioClip note;
    private AudioSource audioSource;

    private RawImage key;
    private Color keyColor;
    public Color hoverColor;

    // Start is called before the first frame update
    void Start() {
        key = GetComponent<RawImage>();
        keyColor = key.color;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = note;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        key.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData) {
        key.color = keyColor;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        audioSource.Play();
    }

}
