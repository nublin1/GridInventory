using UnityEngine;
using NaughtyAttributes;

/*
* IileName: им€ по умолчанию при создании ассета.
* menuName: им€ ассета, отображаемое в Asset Menu.
* order: место размещени€ ассета в Asset Menu. Unity раздел€ет ассеты на подгруппы с множителем 50. “о есть значение 51 поместит новый ассет во вторую группу Asset Menu.
*/

namespace GridInventorySystem
{
    [CreateAssetMenu(fileName = "New InventoryItem", menuName = "InventoryItem", order = 51)]
    public class InventoryItemData : ScriptableObject
    {
        [ShowNonSerializedField]        
        private string m_Id;

        [SerializeField]
        private string itemName;
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;

        #region containerOptions
        [SerializeField]
        private bool isContainer;

        [ShowIf("isContainer")]
        [SerializeField]
        private GameObject pf_ItemContainer;
        private Transform itemContainer;
        #endregion

        [SerializeField]
        [Range(1, 100)]
        private int m_Stack = 1;

        [SerializeField]
        [Range(1, 999f)]
        private int m_maxStack;

        #region Properties
        public string Id { get => m_Id; set => m_Id = value; }
        public string ItemName { get => itemName; }
        public Sprite Icon { get => icon; }
        public int Width { get => width; }
        public int Height { get => height; }       
        public bool IsContainer { get => isContainer;}
        public GameObject Pf_ItemContainer { get => pf_ItemContainer; }
        public Transform ItemContainer { get => itemContainer; set => itemContainer = value; }
        public int Stack { get => m_Stack; set => m_Stack = value; }
        #endregion

        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(this.m_Id))
            {
                this.m_Id = System.Guid.NewGuid().ToString();
            }
        }
    }
}