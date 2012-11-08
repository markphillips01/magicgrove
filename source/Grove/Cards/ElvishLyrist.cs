﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Details.Cards.Costs;
  using Core.Details.Cards.Effects;
  using Core.Details.Mana;
  using Core.Dsl;
  using Core.Targeting;

  public class ElvishLyrist : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Elvish Lyrist")
        .ManaCost("{G}")
        .Type("Creature Elf")
        .Text("{G},{T}, Sacrifice Elvish Lyrist: Destroy target enchantment.")
        .FlavorText(
          "Bring the spear of ancient briar;{EOL}Bring the torch to light the pyre.{EOL}Bring the one who trod our ground;{EOL}Bring the spade to dig his mound.")
        .Power(1)
        .Toughness(1)
        .Timing(Timings.Creatures())
        .Abilities(
          ActivatedAbility(
            "{G},{T}, Sacrifice Elvish Lyrist: Destroy target enchantment.",
            Cost<TapAndSacOwnerPayMana>(cost => cost.Amount = "{G}".ParseManaAmount()),
            Effect<DestroyTargetPermanents>(),
            timing: Timings.InstantRemovalTarget(),
            effectValidator: Validator(Validators.Permanent(card => card.Is().Enchantment)),
            selectorAi: TargetSelectorAi.OrderByDescendingScore()            
            )
        );
    }
  }
}