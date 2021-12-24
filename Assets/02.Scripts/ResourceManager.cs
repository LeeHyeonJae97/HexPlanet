using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private int Gold;
    private int Coal;

    public StatusBar StatusBar;

    public void CollectGold(int amount)
    {
        Gold += amount;
        StatusBar.SetGold(Gold);
    }

    public void CollectCoal(int amount)
    {
        Coal += amount;
        StatusBar.SetCoal(Coal);
    }

    public bool ConsumeGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            StatusBar.SetGold(Gold);

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ConsumeCoal(int amount)
    {
        if (Coal >= amount)
        {
            Coal -= amount;
            StatusBar.SetCoal(amount);

            return true;
        }
        else
        {
            return false;
        }
    }
}
