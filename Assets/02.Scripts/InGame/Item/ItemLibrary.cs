using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Libraries/ItemLibrary")]
public class ItemLibrary : ScriptableObject
{
    public List<ItemEffect> templates = new List<ItemEffect>();
}
