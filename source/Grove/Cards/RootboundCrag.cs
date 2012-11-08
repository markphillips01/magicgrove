﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using System.Linq;
  using Core;
  using Core.Ai;
  using Core.Details.Cards.Effects;
  using Core.Details.Mana;
  using Core.Dsl;
  using Infrastructure;

  public class RootboundCrag : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Rootbound Crag")
        .Type("Land")
        .Text(
          "Rootbound Crag enters the battlefield tapped unless you control a Mountain or a Forest.{EOL}{T}: Add {R} or {G} to your mana pool.")
        .Abilities(
          ManaAbility(
            new ManaUnit(ManaColors.Red | ManaColors.Green),
            "{T}: Add {R} or {G} to your mana pool."
            ))
        .Effect<PutIntoPlay>(e => e.PutIntoPlayTapped = e.Controller.Battlefield.None(card => card.Is("forest") || card.Is("mountain")))
        .Timing(Timings.Lands());
    }
  }
}