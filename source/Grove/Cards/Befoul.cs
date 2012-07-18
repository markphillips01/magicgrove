﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Details.Cards.Effects;
  using Core.Details.Mana;
  using Core.Dsl;
  using Core.Targeting;

  public class Befoul : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Befoul")
        .ManaCost("{2}{B}{B}")
        .Type("Sorcery")
        .Text("Destroy target land or nonblack creature. It can't be regenerated.")
        .FlavorText("'The land putrefied at its touch, turned into an oily bile in seconds.'{EOL}—Radiant, archangel")
        .Effect<DestroyTargetPermanent>(p => p.AllowRegenerate = false)
        .Timing(Timings.FirstMain())
        .Category(EffectCategories.Destruction)
        .Targets(
          filter: TargetFilters.Destroy(),
          effect: C.Selector(Selectors.Permanent(card =>
            card.Is().Land || (card.Is().Creature && !card.HasColor(ManaColors.Black)))));
    }
  }
}