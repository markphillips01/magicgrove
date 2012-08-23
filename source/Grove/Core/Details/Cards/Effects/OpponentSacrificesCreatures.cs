﻿namespace Grove.Core.Details.Cards.Effects
{
  using Controllers;

  public class OpponentSacrificesCreatures : Effect
  {
    public int Count { get; set; }

    protected override void ResolveEffect()
    {
      var opponent = Players.GetOpponent(Controller);

      Decisions.Enqueue<SacrificePermanents>(
        controller: opponent,
        init: p =>
          {
            p.Count = Count;
            p.PermanentType = "creature";
          });
    }
  }
}