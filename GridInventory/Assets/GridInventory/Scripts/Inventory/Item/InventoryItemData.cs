using UnityEngine;
using NaughtyAttributes;

/*
* IileName: им€ по умолчанию при создании ассета.
* menuName: им€ ассета, отображаемое в Asset Menu.
* order: место размещени€ ассета в Asset Menu. Unity раздел€ет ассеты на подгруппы с множителем 50. “о есть значение 51 поместит новый ассет во вторую группу Asset Menu.
*/

namespace GridInventory
{
    [CreateAssetMenu(fileName = "New InventoryItem", menuName = "InventoryItem", order = 51)]
    public class InventoryItemData : ScriptableObject
    {
        [SerializeField]
        private string itemName;
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;

        [SerializeField]
        private bool isContainer;

        [ShowIf("isContainer")]
        [SerializeField]
        private GameObject itemContainer;

        public string ItemName { get => itemName; }
        public Sprite Icon { get => icon; }
        public int Width { get => width; }
        public int Height { get => height; }       
        public bool IsContainer { get => isContainer;}
        public GameObject ItemContainer { get => itemContainer; }
    }   
}