using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{

    private Button m_Button;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
       
    }
    private void OnEnable() => m_Button.onClick.AddListener(OnButtonClicked);
   

    private void OnDisable() => m_Button.onClick.RemoveListener(OnButtonClicked);

    private void OnButtonClicked()
    {
        AudioManager.Instance.PlaySFX(SoundType.ClickedButton);
    }
}
