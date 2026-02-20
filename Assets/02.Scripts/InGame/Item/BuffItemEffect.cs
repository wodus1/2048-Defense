using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Item/Buff")]
public class BuffItemEffect : ItemEffect, IDurationItem // 버프형 아이템
{
    [SerializeField] private List<Effect> effect;
    [SerializeField] private float duration = 5f;

    public float Duration => duration;

    public override void Execute(ItemUseContext itemUseContext)
    {
        if (itemUseContext.PlayerStatsSystem != null)
            itemUseContext.PlayerStatsSystem.StartCoroutine(BuffExecute(itemUseContext));
    }

    private IEnumerator BuffExecute(ItemUseContext itemUseContext)
    {
        PlayerStatsSystem playerStatsSystem = itemUseContext.PlayerStatsSystem;
        object handle = itemUseContext.Handle;

        foreach (Effect effect in effect)
            effect.Apply(playerStatsSystem, handle);

        yield return new WaitForSeconds(duration);

        playerStatsSystem.RemoveMultiplierEffect(handle);
    }
}