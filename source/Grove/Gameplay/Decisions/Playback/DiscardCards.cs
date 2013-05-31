﻿namespace Grove.Gameplay.Decisions.Playback
{
  using Results;

  public class DiscardCards : Decisions.DiscardCards
  {
    protected override bool ShouldExecuteQuery { get { return true; } }

    protected override void ExecuteQuery()
    {
      Result = (ChosenCards) Game.LoadDecisionResult();
    }
  }
}