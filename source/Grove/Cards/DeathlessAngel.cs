﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Ai.TargetingRules;
  using Core.Costs;
  using Core.Dsl;
  using Core.Effects;
  using Core.Mana;
  using Core.Modifiers;

  public class DeathlessAngel : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Deathless Angel")
        .ManaCost("{4}{W}{W}")
        .Type("Creature Angel")
        .Text("{Flying}{EOL}{W}{W}: Target creature is indestructible this turn.")
        .FlavorText(
          "'I should have died that day, but I suffered not a scratch. I awoke in a lake of blood, none of it apparently my own.'{EOL}—The War Diaries")
        .Power(5)
        .Toughness(7)
        .StaticAbilities(Static.Flying)
        .ActivatedAbility(p =>
          {
            p.Text = "{W}{W}: Target creature is indestructible this turn.";
            p.Cost = new PayMana("{W}{W}".ParseMana(), ManaUsage.Abilities);

            p.Effect = () => new ApplyModifiersToTargets(
              () => new AddStaticAbility(Static.Indestructible)) {Category = EffectCategories.Protector};

            p.TargetSelector.AddEffect(trg => trg.Is.Creature().On.Battlefield());
            p.TargetingRule(new GainIndestructible());
          });
    }
  }
}