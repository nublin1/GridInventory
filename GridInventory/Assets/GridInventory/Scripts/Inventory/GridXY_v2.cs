
using UnityEngine;

public class GridXY_v2 
{
    private int _gridWidth;
    private int _gridHeight;
    private float _cellSize = 1;
    private Vector3 _originalPosition = Vector3.zero;

    #region getters
    public Vector3 OriginalPosition { get => _originalPosition; set => _originalPosition = value; }
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

    public void GetXY(Vector2 mousePoistion, out int x, out int y)
    {
        Vector2 localPosition = Vector2.zero;
        localPosition.x =  mousePoistion.x - _originalPosition.x;
        localPosition.y = mousePoistion.y - _originalPosition.y;

        Debug.Log(_originalPosition);

        x = Mathf.FloorToInt(localPosition.x / _cellSize);
        y = Mathf.FloorToInt(localPosition.y / _cellSize );
    }
}
