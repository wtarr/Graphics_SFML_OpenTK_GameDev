using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFMLStarter2;

namespace Game
{
    public class Game
    {
        private double _timePerFrame = 1/60f;
        private RenderWindow _window;
        private CircleShape shape;
        private Font statsFont;
        private Text text;
        private Stopwatch stopwatch;
        private double _statisticsUpdateTime;
        private double _statisticsNumFrames;
        private Texture _character;
        private Animation animation;
        private Vertex[] line = new Vertex[2];


        public Game()
        {
            _window = new RenderWindow(new VideoMode(600, 600), "SFML.NET Window");
            _window.Closed += OnClose;
            _window.KeyPressed += OnKeyPressed;
            _window.SetFramerateLimit(60);

            Init();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                _window.Close();
        }


        private void Init()
        {
            shape = new CircleShape(100f) {FillColor = Color.Red};
            shape.Position = new Vector2f(_window.Size.X/2f - shape.Radius, _window.Size.Y/2f - shape.Radius);
            try
            {
                // http://www.dafont.com/sansation.font
                statsFont = new Font("Content/Font/Sansation.ttf");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to load font");
                Console.ReadKey();
                _window.Close();
            }

            text = new Text("Loading stats...", statsFont)
            {
                Position = new Vector2f(5f, 5f), 
                CharacterSize = 14
            };

            // Load the character
            _character = new Texture("Content/Texture/Test.png");

            animation = new Animation(_character);
            animation.SetFrameSize(new Vector2i(128, 128));
            animation.SetNumFrames(3);
            animation.SetDuration(2);
            animation.SetRepeating(true);
            animation.SetPosition(new Vector2f(_window.Size.X / 2f, _window.Size.Y / 2f));

            //line[0] = new Vertex(new Vector2f(10, 10));
            //line[1] = new Vertex(new Vector2f(130, 130));
            
        }

        public void Run()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeSinceLastUpdate = 0;
            double elapsedTime;

            while (_window.IsOpen())
            {
                elapsedTime = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                timeSinceLastUpdate += elapsedTime;

                while (timeSinceLastUpdate > _timePerFrame)
                {
                    timeSinceLastUpdate -= _timePerFrame;
                    _window.DispatchEvents();
                    Update(_timePerFrame);
                }

                UpdateStatistics(elapsedTime);
                Render();

            }
        }

        private void Update(double delta)
        {
            animation.Update(_timePerFrame);
        }

        private void Render()
        {
            _window.Clear();
            //_window.Draw(shape);
            _window.Draw(text);
            animation.Draw(_window, RenderStates.Default);
            //_window.Draw(line, 0, 2, PrimitiveType.Lines);
            _window.Display();
        }

        private void UpdateStatistics(double elapsedTime)
        {
            _statisticsUpdateTime += elapsedTime;
            _statisticsNumFrames += 1;

            if (_statisticsUpdateTime >= 1)
            {
                text.DisplayedString = "Frames / Second = " + _statisticsNumFrames + "\n" +
                                       "Time / Update = " + string.Format("{0:0}", (_statisticsUpdateTime * 1000000) / _statisticsNumFrames) + "us";

                _statisticsUpdateTime -= 1;
                _statisticsNumFrames = 0;
            }

        }

        private void OnClose(object sender, EventArgs e)
        {
            var window = (RenderWindow) sender;
            window.Close();
        }

    }


}
