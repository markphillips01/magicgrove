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

  public class RuneofProtectionBlack : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Rune of Protection: Black")
        .ManaCost("{1}{W}")
        .Type("Enchantment")
        .Text(
          "{W}: The next time a black source of your choice would deal damage to you this turn, prevent that damage.{EOL}Cycling {2} ({2}, Discard this card: Draw a card.)")
        .Timing(Timings.FirstMain())
        .Cycling("{2}")
        .Abilities(
          ActivatedAbility(
            "{W}: The next time a black source of your choice would deal damage to you this turn, prevent that damage.",
            Cost<TapOwnerPayMana>(cost => { cost.Amount = ManaAmount.White; }),
            Effect<PreventDamageFromSourceToController>(e => e.OnlyOnce = true),
            effectValidator: Validator(
              Validators.EffectOrPermanent(target => target.HasColor(ManaColors.Black)), text: "Select a damage source."),
            selectorAi: TargetSelectorAi.PreventDamageFromSourceToController(),
            timing: Timings.NoRestrictions())
        );
    }
  }
}