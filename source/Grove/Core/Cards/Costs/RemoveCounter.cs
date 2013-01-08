﻿namespace Grove.Core.Cards.Costs
{
  using Targeting;

  public class RemoveCounter : Cost
  {
    public override bool CanPay(ref int? maxX)
    {
      return Card.ChargeCountersCount > 0;
    }

    public override void Pay(ITarget target, int? x)
    {
      Card.RemoveChargeCounter();
    }
  }
}