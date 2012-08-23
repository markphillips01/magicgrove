﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Dsl;

  public class SandbarSerpent : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Sandbar Serpent")
        .ManaCost("{4}{U}")
        .Type("Creature - Serpent")
        .Text("{Cycling} {2}({2}, Discard this card: Draw a card.)")
        .FlavorText(
          "Treacherous and unpredictable currents around Tolaria earned the nickname 'serpent wakes.'")
        .Power(3)
        .Toughness(4)
        .Timing(Timings.Creatures())
        .Cycling("{2}");
    }
  }
}