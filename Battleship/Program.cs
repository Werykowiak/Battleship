using Battleship.Model;
using Battleship.View;
using Battleship.Controller;
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
