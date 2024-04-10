using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class VariableMatrix<T>
{
    private List<List<T>> matrix = new List<List<T>>();

    private int row;
    private int column;

    public int Row
    {
        get
        {
            return row = matrix.Count;
        }
    }

    public int Column
    {
        get
        {
            if (row > 0) column = matrix[0].Count;
            return column;
        }
    }

    public VariableMatrix() {}

    public VariableMatrix(int row, int column)
    {
        for (int i = 0; i < row; i++)
        {
            AddRow();
            matrix[i].Capacity = column;
        }
    }

    public void AddRow()
    {
        matrix.Add(new List<T>());
    }

    public void AddElement(int rowIndex, T element)
    {
        if (rowIndex < matrix.Count)
        {
            matrix[rowIndex].Add(element);
        }
        else
        {
            int rowDiff = rowIndex - matrix.Count + 1;
            for (int i = 0; i < rowDiff; i++) AddRow();
            matrix[rowIndex].Add(element);
        }
    }

    public T GetElement(int rowIndex, int columnIndex)
    {
        if (rowIndex < matrix.Count && columnIndex < matrix[rowIndex].Count)
        {
            return matrix[rowIndex][columnIndex];
        }
        else
        {
            throw new IndexOutOfRangeException("Index out of range");
        }
    }

    public void SetElement(int rowIndex, int columnIndex, T value)
    {
        if (rowIndex < matrix.Count && columnIndex < matrix[rowIndex].Count)
        {
            matrix[rowIndex][columnIndex] = value;
        }
        else
        {
            throw new IndexOutOfRangeException("Index out of range");
        }
    }

    public bool Contains(T value)
    {
        foreach (List<T> row in matrix)
        {
            if (row.Contains(value))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsAny(List<T> items)
    {
        foreach (T item in items)
        {
            if (Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsAll(List<T> values)
    {
        foreach (T value in values)
        {
            if (!Contains(value))
            {
                return false;
            }
        }
        return true;
    }

    public void PrintMatrix()
    {
        StringBuilder sb = new StringBuilder();

        foreach (List<T> row in matrix)
        {
            foreach (T item in row)
            {
                sb.Append(item.ToString() + ", ");
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }
}