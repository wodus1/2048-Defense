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
        FXSystem fXSystem = itemUseContext.FXSystem;
        object handle = itemUseContext.Handle;

        foreach (Effect effect in effect)
            effect.Apply(playerStatsSystem, handle);

        FXInstance particle = null;
        if (Particle != null)
            particle = fXSystem.PlayLoop(Particle, playerStatsSystem.buffFXRect, new Vector2(0, -10f), Quaternion.identity);

        yield return new WaitForSeconds(duration);

        playerStatsSystem.RemoveMultiplierEffect(handle);

        if(particle != null)
            fXSystem.StopLoop(particle);
    }
}