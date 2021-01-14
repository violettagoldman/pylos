using System;
using System.Collections;
using System.Collections.Generic;

namespace Pylos {
  public class Stage
  {
    private Stage _above;
    private Stage _below;
    public Ball[,] TabBall { get; }
    public int Size { get; }


    public Stage Above
    {
      get => _above;
      set => _above = value;
    }

    public Stage Below
    {
      get => _below;
      set => _below = value;
    }

    public Ball GetBall(int i, int j)
    {
      return TabBall[i, j];
    }


    public Stage(int t){
      Size = t;
      _above = null;
      _below = null;
      TabBall = new Ball [t,t];
      for(var i = 0 ; i<t ; i++){
        for(var j = 0 ; i<t ; i++){
          TabBall[i,j] = null;
        }
      }
    }

    public Ball GetEmplacement(int i, int j){ return TabBall[i,j] ;}

    public void DisplayStage(){
      for(int i = 0 ; i<Size ; i++){
        for(int j = 0 ; j<Size ; j++){
          if(TabBall[i,j]==null){
            Console.BackgroundColor = ConsoleColor.Black;
          }else if(TabBall[i,j].Player.Number==0){
            Console.BackgroundColor = ConsoleColor.Red;
          }else {
            Console.BackgroundColor = ConsoleColor.Blue;
          }
          Console.Write("[ ]");  
        }
        Console.BackgroundColor = ConsoleColor.Black;
      Console.Write(Environment.NewLine);
      }
    }
    
    public void AddBall(Ball b, int i, int j)
    {
        TabBall[i,j]=b;
    }

     public Ball RemoveBall(int i, int j)
    {   
        Ball b = TabBall[i,j];
        TabBall[i,j]=null;
        return b;
    }

    public virtual bool EmplacementEmpty(int i, int j) => CheckEmplacementExist(i,j) && TabBall[i,j]==null;

    public bool CheckBelow(int i, int j)
    {
      return !_below.EmplacementEmpty(i,j) 
                   &&!_below.EmplacementEmpty(i+1,j) 
                   &&!_below.EmplacementEmpty(i,j+1) 
                   &&!_below.EmplacementEmpty(i+1,j+1);
    }
    

    public bool CheckAbove(int i, int j) => _above!=null && (_above.EmplacementEmpty(i,j) || !_above.CheckEmplacementExist(i,j))
                          && (_above.EmplacementEmpty(i-1,j) || !_above.CheckEmplacementExist(i-1,j))
                          && (_above.EmplacementEmpty(i,j-1) || !_above.CheckEmplacementExist(i,j-1))
                          && (_above.EmplacementEmpty(i-1,j-1) || !_above.CheckEmplacementExist(i-1,j-1));

    public bool PlayIsPossible(int i, int j)
    {
      if(EmplacementEmpty(i,j)  && CheckBelow(i,j)){
        Console.WriteLine("c'est possible de jouer là !");
        return true;
      }
      Console.WriteLine("c'est impossible de jouer là !");
      return false;
    }
    
    public bool Square(int i, int j) =>  FindSquare(i-1,j-1) || FindSquare(i-1,j)||FindSquare(i,j-1)||FindSquare(i,j);
    

    public bool FindSquare(int i, int j) => CheckSamePlayer(i,j,i+1,j) && CheckSamePlayer(i,j,i,j+1) && CheckSamePlayer(i,j,i+1,j+1);
    

    public bool CheckSamePlayer(int i, int j, int i2, int j2) => (CheckEmplacementExist(i,j) && CheckEmplacementExist(i2,j2)
                                                                 && TabBall[i, j]!= null && TabBall[i2, j2]!= null 
                                                                 &&TabBall[i,j].SamePlayer(TabBall[i2,j2]));
    
    public bool CheckEmplacementExist(int i, int j)=> (i>=0 && j>=0 && i<Size && j<Size);

    public void AllPlaysPossibles(List<Position> list, int k)
    {
      for(var i = 0 ; i<Size ; i++){
        for(var j = 0 ; j<Size ; j++)
        {
          if (!PlayIsPossible(i, j)) continue;
          list.Add(new Position(k,i,j));
          Console.WriteLine("emplacement ajouté");
        }
      }
    }

    public void CanJump(List<Position> list, int k, Player p)
    {
      for(var i = 0 ; i<Size ; i++){
        for(var j = 0 ; j<Size ; j++)
        {
          if (TabBall[i, j] == null || !CheckAbove(i, j) || TabBall[i, j].Player!=p) continue;
          list.Add(new Position( k,i,j));
          Console.WriteLine("ball ajoutée (elle peut monter) :"+i+", "+j);
        }
      }
    }
  }
}