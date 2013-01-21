﻿namespace Grove.Core
{
  using Infrastructure;
  using Targeting;

  [Copyable]
  public class ActivationParameters
  {
    public Targets Targets = new Targets();
    public bool PayCost = true;
    public bool SkipStack;
    public int? X;    
  }
}