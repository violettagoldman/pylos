using System;

namespace Pylos {
public class Board : Stage
  {
      public Board(int t)
        : base(t) 
      { }

      public override bool EmplacementEmpty(int i, int j){
        return false;
      }
      
  }
}