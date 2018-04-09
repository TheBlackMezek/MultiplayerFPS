using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericList<T> where T : class
{


    public int Capacity
    {
        get
        {
            return arr.Length;
        }
        set
        {
            Resize(value);
        }
    }

    public int Count
    {
        get
        {
            return count;
        }
    }

    private T[] arr;
    private int count;



    public GenericList()
    {
        arr = new T[10];
        count = 0;
    }

    public GenericList(int l)
    {
        arr = new T[l];
        count = 0;
    }

    public void Add(T t)
    {
        if(count + 1 >= arr.Length)
        {
            Resize(arr.Length * 2);
        }

        arr[count] = t;
        ++count;
    }

    public T At(int idx)
    {
        return arr[idx];
    }

    public bool Remove(T element)
    {
        for(int i = 0; i < count; ++i)
        {
            if(arr[i] == element)
            {
                for(int n = i + 1; n < count; ++n)
                {
                    arr[n - 1] = arr[n];
                }
                --count;
                return true;
            }
        }
        return false;
    }



    private void Resize(int l)
    {
        T[] newarr = new T[l];

        for(int i = 0; i < l && i < arr.Length; ++i)
        {
            newarr[i] = arr[i];
        }

        arr = new T[l];

        for (int i = 0; i < l; ++i)
        {
            arr[i] = newarr[i];
        }

        if(l < count)
        {
            count = 0;
        }
    }

}
