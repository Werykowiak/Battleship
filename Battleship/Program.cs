namespace Battleship
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);
            controller.Run();
        }
    }
}
