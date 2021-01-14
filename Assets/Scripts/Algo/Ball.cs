namespace Pylos {
  public class Ball
  {
      private readonly Player _player;
      private Stage _stage;
      private int _state;
      private Position _position;

      public Player Player => _player;

      public Position Position
      {
        get => _position;
        set => _position = value;
      }


      public Ball(Player p, int nbP, Position position)
      {
          _player = p;
          _position = position;
          _stage = null;
          _state = 0;
      }

      public Stage GetStage(){ return _stage; }
      public int GetState(){ return _state; }
  
      public void ToPlace(Stage s)
      {
        _stage = s;
        _state = 1;
        _player.PlaceBall();
      }
      
      public void ToRemove()
      {
        _stage = null;
        _state = 0;
        _player.RemoveBall();
      }
      
      public bool SamePlayer(Ball b)
      {
        return _player == b.Player;
      }
      
      
  }
}
