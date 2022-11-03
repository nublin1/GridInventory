
using UnityEngine;

public class GridXY_v2 
{
    private int _gridWidth;
    private int _gridHeight;
    private float _cellSize = 1;
    private Vector3 _originalPosition = Vector3.zero;

    public GridXY_v2(int gridWidth, int gridHeight, float cellSize)
    {
        _gridWidth = gridWidth;
        _gridHeight = gridHeight;
        _cellSize = cellSize;
    }

    #region getters
    public Vector3 OriginalPosition { get => _originalPosition; set => _originalPosition = value; }
    public int GridWidth            { get => _gridWidth; set => _gridWidth = value; }
    public int GridHeight           { get => _gridHeight; set => _gridHeight = value; }
    #endregion

    public void GenerateCells(int width, int height, float cellSize,
           GameObject cellPrefab, Transform cellsContainer)
    {
        _gridWidth = width;
        _gridHeight = height;
        _cellSize = cellSize;        

        // generate cells
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                GameObject cellVisual = GameObject.Instantiate(cellPrefab);
                cellVisual.transform.name = "Cell";
                cellVisual.transform.SetParent(cellsContainer.transform, true);
                cellVisual.transform.localPosition = new Vector3(0f, 0f, 0f);
                cellVisual.transform.localRotation = Quaternion.identity;
                cellVisual.transform.localScale = new Vector3(1f, 1f, 1f);

                Vector2 sizeOfCell = new Vector2(cellSize, cellSize);
                cellVisual.GetComponent<RectTransform>().sizeDelta = sizeOfCell;
                //cellVisual.transform.Find("SlotBackground").GetComponent<RectTransform>().sizeDelta = sizeOfCell;
                //cellVisual.transform.Find("SlotOutline").GetComponent<RectTransform>().sizeDelta = sizeOfCell;
            }
        }

        Vector2 rrr = cellsContainer.GetChild(0).GetComponent<RectTransform>().rect.center;
        _originalPosition = cellsContainer.GetChild(0).GetComponent<RectTransform>().TransformPoint(rrr);
    }    

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * _cellSize + _originalPosition;
    }

    public void GetCellXY(RectTransform rectTransform, Vector2 mousePoistion, out int x, out int y)
    {
        Vector2 localPosition = Vector2.zero;      
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePoistion, null, out localPosition);

        x = Mathf.FloorToInt(localPosition.x / _cellSize);
        y = Mathf.FloorToInt(localPosition.y / _cellSize);
    }

    // Return true if cell is OutOfBounds
    private bool OutOfBoundsCheck(Vector2Int cellPosXY)
    {
        if (cellPosXY.x >= _gridWidth || cellPosXY.y >= _gridHeight || cellPosXY.x < 0 || cellPosXY.y < 0)
            return false;

        return true;
    }
}
