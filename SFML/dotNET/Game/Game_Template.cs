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
    public class Game_Template
    {
        private double _timePerFrame = 1/60f;
        private RenderWindow _window;
        private Stopwatch stopwatch;
        private Font _statsFont;
        private Text _text;
        private double _statisticsUpdateTime;
        private double _statisticsNumFrames;


        public Game_Template()
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
            try
            {
                // http://www.dafont.com/sansation.font
                _statsFont = new Font("Content/Font/Sansation.ttf");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to load font");
                _window.Close();
            }

            _text = new Text("Loading stats...", _statsFont)
            {
                Position = new Vector2f(5f, 5f),
                CharacterSize = 14
            };
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

                    // process events
                    _window.DispatchEvents();
                    Update(_timePerFrame);
                }

                UpdateStatistics(elapsedTime);
                Render();

            }
        }

        private void Update(double delta)
        {
            
        }

        private void Render()
        {
            _window.Clear();
            // Draw calls
            _window.Draw(_text);

            _window.Display();
        }

        private void UpdateStatistics(double elapsedTime)
        {
            _statisticsUpdateTime += elapsedTime;
            _statisticsNumFrames += 1;

            if (_statisticsUpdateTime >= 1)
            {
                _text.DisplayedString = "Frames / Second = " + _statisticsNumFrames + "\n" +
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
