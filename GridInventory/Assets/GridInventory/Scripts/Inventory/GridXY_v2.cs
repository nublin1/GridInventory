
using UnityEngine;

public class GridXY_v2 
{
    private int _width;
    private int _height;
    private float _cellSize = 1;
    private Vector3 _originalPosition = Vector3.zero;

    public void GenerateCells(int width, int height, float cellSize, Vector3 originalPosition,
           GameObject cellPrefab, Transform cellsContainer)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originalPosition = originalPosition;

        // generate cells
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GameObject cellVisual = GameObject.Instantiate(cellPrefab);
                cellVisual.transform.name = "Cell";
                cellVisual.transform.SetParent(cellsContainer.transform, true);
                cellVisual.transform.localPosition = new Vector3(0f, 0f, 0f);
                cellVisual.transform.localRotation = Quaternion.identity;
                cellVisual.transform.localScale = new Vector3(1f, 1f, 1f);

                Vector2 sizeOfCell = new Vector2(cellSize, cellSize);
                cellVisual.GetComponent<RectTransform>().sizeDelta = sizeOfCell;
                cellVisual.transform.Find("SlotBackground").GetComponent<RectTransform>().sizeDelta = sizeOfCell;
                cellVisual.transform.Find("SlotOutline").GetComponent<RectTransform>().sizeDelta = sizeOfCell;
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * _cellSize + _originalPosition;
    }
}
