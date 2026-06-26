using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class UIPetItem : MonoBehaviour
{
    [SerializeField] private Image m_AvatarImage;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_CountText;
    [SerializeField] private Button m_ActionButton;

    [Inject] private SignalBus m_SignalBus;
    public string ItemName { get; private set; }

    public void InitAndSetup(AnimalGroup groupData)
    {
        if (groupData == null)
        {
            m_NameText?.SetText(string.Empty);
            m_CountText?.SetText("0");
            return;
        }

        ItemName = groupData.groupName;
        m_NameText?.SetText(ItemName);

        int totalCount = groupData.animals != null ? groupData.animals.Count : 0;
        m_CountText?.SetText(totalCount.ToString());

        if (m_AvatarImage != null && !string.IsNullOrEmpty(ItemName))
        {
            Sprite loadedSprite = Resources.Load<Sprite>($"Avatar/{ItemName}");
            if (loadedSprite != null) m_AvatarImage.sprite = loadedSprite;
        }

        m_ActionButton?.onClick.RemoveAllListeners();
        m_ActionButton?.onClick.AddListener(() => OnButtonAddAnimal());
    }

    private void OnButtonAddAnimal()
    {
        if (m_ActionButton != null) m_ActionButton.interactable = false;
        m_SignalBus.Fire(new AddAnimalSignal(ItemName)); // Fire signal to AnimalController
    }
    private void OnAddAnimalResult(AddAnimalResultSignal signal)
    {
        if (signal.GroupName != ItemName) return;
        if (m_ActionButton != null)
            m_ActionButton.interactable = true;

    }

    public void OnSpawned()
    {
        gameObject.SetActive(true);
        if (m_ActionButton != null) m_ActionButton.interactable = true;
        m_SignalBus.Subscribe<AddAnimalResultSignal>(OnAddAnimalResult);
    }

    public void OnDespawned()
    {
        m_ActionButton?.onClick.RemoveAllListeners();
        m_SignalBus.Unsubscribe<AddAnimalResultSignal>(OnAddAnimalResult);
        gameObject.SetActive(false);
    }

    public class Pool : MonoMemoryPool<UIPetItem>
    {
        protected override void OnSpawned(UIPetItem item) => item.OnSpawned();
        protected override void OnDespawned(UIPetItem item) => item.OnDespawned();
    }
}
