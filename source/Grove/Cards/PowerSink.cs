﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai.CostRules;
  using Core.Dsl;
  using Core.Effects;
  using Core.Mana;

  public class PowerSink : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Power Sink")
        .ManaCost("{U}").HasXInCost()
        .Type("Instant")
        .Text(
          "Counter target spell unless its controller pays X. If he or she doesn't, that player taps all lands with mana abilities he or she controls and empties his or her mana pool.")
        .Cast(p =>
          {
            p.Effect = () => new CounterTargetSpell(
              doNotCounterCost: P(e => e.X.GetValueOrDefault()),
              tapLandsAndEmptyManaPool: true);

            p.TargetSelector.AddEffect(trg => trg.Is.CounterableSpell().On.Stack());

            p.CostRule(new CounterUnlessOpponentPaysX());
            p.TimingRule(new Core.Ai.TimingRules.Counterspell());
            p.TargetingRule(new Core.Ai.TargetingRules.Counterspell());
          });
    }
  }
}