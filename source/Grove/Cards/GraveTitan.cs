﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Details.Cards;
  using Core.Details.Cards.Effects;
  using Core.Details.Cards.Triggers;
  using Core.Details.Mana;
  using Core.Dsl;
  using Core.Zones;

  public class GraveTitan : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Grave Titan")
        .ManaCost("{4}{B}{B}")
        .Type("Creature - Giant")
        .Text(
          "{Deathtouch}{EOL}Whenever Grave Titan enters the battlefield or attacks, put two 2/2 black Zombie creature tokens onto the battlefield.")
        .FlavorText("Death in form and function.")
        .Power(6)
        .Toughness(6)
        .Timing(Timings.Creatures())
        .Abilities(
          Static.Deathtouch,
          TriggeredAbility(
            "Whenever Grave Titan enters the battlefield or attacks, put two 2/2 black Zombie creature tokens onto the battlefield.",
            L(Trigger<OnZoneChange>(t => { t.To = Zone.Battlefield; }),
              Trigger<OnAttack>()),
            Effect<CreateTokens>(e =>
              {
                e.Tokens(
                  Card
                    .Named("Zombie Token")
                    .FlavorText(
                      "'Your brain is rotting?!.'{EOL}'...enough.'{EOL}-Y.A, 'The seven zombies'")
                    .Power(2)
                    .Toughness(2)
                    .Type("Creature - Token - Zombie")
                    .Colors(ManaColors.Black)
                  );
                e.Count = 2;
              })));
    }
  }
}