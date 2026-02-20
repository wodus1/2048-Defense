using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Libraries/ItemLibrary")]
public class ItemLibrary : ScriptableObject // 아이템 effect 라이브러리
{
    public List<ItemEffect> templates = new List<ItemEffect>();
}
