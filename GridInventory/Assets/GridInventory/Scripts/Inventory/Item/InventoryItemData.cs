using UnityEngine;

/*
* IileName: ��� �� ��������� ��� �������� ������.
* menuName: ��� ������, ������������ � Asset Menu.
* order: ����� ���������� ������ � Asset Menu. Unity ��������� ������ �� ��������� � ���������� 50. �� ���� �������� 51 �������� ����� ����� �� ������ ������ Asset Menu.
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

        public string ItemName { get => itemName; }
        public Sprite Icon { get => icon; }
        public int Width { get => width; }
        public int Height { get => height; }
    }   
}