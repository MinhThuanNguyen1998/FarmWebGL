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

    [Inject] private UserDataService m_UserDataService;
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
        m_ActionButton?.onClick.AddListener(() => OnButtonAddAnimal(groupData).Forget());
    }

    private async UniTaskVoid OnButtonAddAnimal(AnimalGroup groupData)
    {
        if (m_UserDataService == null)
        {
            Debug.LogError("UserDataService is not injected properly!");
            return;
        }

        // Disable button interaction to prevent spamming network requests during processing
        if (m_ActionButton != null) m_ActionButton.interactable = false;

        // Call the service to process the animal addition via backend API[cite: 1, 2]
        bool isSuccess = await m_UserDataService.AddAnimalAsync(groupData.groupName);

        // Re-enable the action button only if the operation failed[cite: 1]
        if (!isSuccess && m_ActionButton != null)
        {
            m_ActionButton.interactable = true;
        }
    }

    public void OnSpawned()
    {
        gameObject.SetActive(true);
        if (m_ActionButton != null) m_ActionButton.interactable = true;
    }

    public void OnDespawned()
    {
        m_ActionButton?.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    public class Pool : MonoMemoryPool<UIPetItem>
    {
        protected override void OnSpawned(UIPetItem item) => item.OnSpawned();
        protected override void OnDespawned(UIPetItem item) => item.OnDespawned();
    }
}
