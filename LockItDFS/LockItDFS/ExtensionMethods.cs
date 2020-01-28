using System.Collections;
using System.Collections.Generic;

namespace LockItDFS
{
    public static class ExtensionMethods
    {
        public static List<List<int>> CreateClone(this List<List<int>> list)
        {
            var clonedList = new List<List<int>>();

            list.ForEach((item) => { clonedList.Add(new List<int>(item)); });

            return clonedList;
        }
    }
}