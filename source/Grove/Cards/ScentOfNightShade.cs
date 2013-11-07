﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using System.Linq;
  using Artifical;
  using Artifical.TargetingRules;
  using Artifical.TimingRules;
  using Gameplay.Characteristics;
  using Gameplay.Effects;
  using Gameplay.Misc;

  public class ScentOfNightShade : CardTemplateSource
  {
    public override IEnumerable<CardTemplate> GetCards()
    {
      yield return Card
        .Named("Scent of Nightshade")
        .ManaCost("{1}{B}")
        .Type("Instant")
        .Text(
          "Reveal any number of black cards in your hand. Target creature gets -X/-X until end of turn, where X is the number of cards revealed this way.")
        .Cast(p =>
          {
            p.Effect = () => new CreatureGetsM1M1ForEachRevealedCard();
            p.TargetSelector.AddEffect(trg => trg.Is.Creature().On.Battlefield());
            p.TargetingRule(new EffectReduceToughness(
              getAmount: tp => tp.Controller.Hand.Count(c => c.HasColor(CardColor.Black))));
            p.TimingRule(new TargetRemovalTimingRule(removalTag: EffectTag.ReduceToughness));
          });
    }
  }
}