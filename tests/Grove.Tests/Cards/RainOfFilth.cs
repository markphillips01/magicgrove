﻿namespace Grove.Tests.Cards
{
  using Infrastructure;
  using Xunit;

  public class RainOfFilth
  {
    public class Ai : AiScenario
    {
      [Fact]
      public void CastTitan()
      {
        Hand(P1, "Grave Titan", "Rain of Filth");
        Battlefield(P1, "Swamp", "Swamp", "Swamp", "Swamp");

        RunGame(3);
        Equal(10, P2.Life);
      }

      [Fact]
      public void BugManaNotAvailableExceptionWhen2Filths()
      {
        // todo

        Hand(P1, "Breach", "Rain of Filth", "Rain of Filth");
        Hand(P2, "Hermetic Study", "Symbiosis", "Forest", "Island", "Hermetic Study");
        Battlefield(P1, "Swamp", "Claws of Gix", "Swamp", "Swamp", "Looming Shade");
        Battlefield(P2, "Island", "Forest", "Pit Trap", "Forest", "Gorilla Warrior");

        RunGame(1);
      }     
    }
  }
}