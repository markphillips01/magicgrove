﻿namespace Grove
{
  using System;

  [Serializable]
  public class SavedMatch
  {
    public int Player1WinCount;
    public int Player2WinCount;
    public SavedGame SavedGame;
    public int? Looser;
  }
}