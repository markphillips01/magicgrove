﻿namespace Grove.Core.Controllers.Human
{
  using System.Linq;
  using Infrastructure;
  using Ui;
  using Ui.SelectTarget;
  using Ui.Shell;

  public class DeclareAttackers : Controllers.DeclareAttackers
  {
    public ViewModel.IFactory DialogFactory { get; set; }
    public Publisher Publisher { get; set; }
    public IShell Shell { get; set; }

    protected override void ExecuteQuery()
    {
      var dialog = DialogFactory.Create(
        new UiTargetSelector(
          minTargetCount: 0,
          maxTargetCount: null,
          text: "Select attackers.",
          isValid: target => target.CanAttack && target.Controller == Player
          ),
        canCancel: false,
        instructions: "(Press spacebar when done.)",
        targetSelected: target => Publisher.Publish(
          new AttackerSelected{
            Attacker = target.Card()
          }),
        targetUnselected: target => Publisher.Publish(
          new AttackerUnselected{
            Attacker = target.Card()
          }));

      Shell.ShowModalDialog(dialog, DialogType.Small, SelectionMode.SelectTarget);

      Result = dialog.Selection.ToList();
    }
  }
}