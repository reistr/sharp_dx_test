// See
// http://web.archive.org/web/20140625070410/http://rastertek.com/tutindex.html
// https://rastertekdx.codeplex.com/

namespace SharpDXGame
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
