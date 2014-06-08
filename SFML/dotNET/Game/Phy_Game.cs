using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using SFMLStarter2;

namespace Game
{
    public class Phy_Game
    {
        private UInt16 _width = 800, _height = 480;
        private double _timePerFrame = 1/60f;
        private RenderWindow _window;
        private Stopwatch stopwatch;
        private Font _statsFont;
        private Text _text;
        private double _statisticsUpdateTime;
        private double _statisticsNumFrames;

        private View _view;
        

        // Physics stuff
        private World _world;

        private const float MeterInPixels = 64f;

        private Vector2 _screenCenter;

        private Body _circleBody;
        private Body _groundBody;

        private CircleShape _circleSprite;
        private RectangleShape _groundSprite;

        private float _diameter = 96f;





        public Phy_Game()
        {
            _window = new RenderWindow(new VideoMode(_width, _height), "SFML.NET Window");
            _window.Closed += OnClose;
            _window.KeyPressed += OnKeyPressed;
            _window.SetFramerateLimit(60);

            _view = _window.DefaultView;
            
            Init();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                _window.Close();
            if (e.Code == Keyboard.Key.A)
                _view.Move(new Vector2f(0, 5));
            if (e.Code == Keyboard.Key.D)
                _view.Move(new Vector2f(0, -5));
                
        }


        private void Init()
        {
            _screenCenter = new Vector2(_width/2, _height/2);
            _world = new World(new Vector2(0, 20));

            _circleSprite = new CircleShape {Radius = _diameter/2f, FillColor = new Color(Color.Blue)};

            _groundSprite = new RectangleShape(new Vector2f(512f, 32f)) {FillColor = new Color(Color.Green)};

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = (_screenCenter / MeterInPixels) + new Vector2(0, -3.5f);

            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, _diameter / (2f * MeterInPixels), 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _circleBody.Restitution = 0.75f;
            _circleBody.Friction = 0.5f;

            /* Ground */
            Vector2 groundPosition = (_screenCenter / MeterInPixels) + new Vector2(0, 3.25f);
            _groundSprite.Position = new Vector2f(
                groundPosition.X * MeterInPixels - (_groundSprite.GetGlobalBounds().Width /2f),
                groundPosition.Y * MeterInPixels - (_groundSprite.GetGlobalBounds().Height/ 2f));

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, 512f / MeterInPixels, 32f / MeterInPixels, 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;

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
            _world.Step((float)delta);

            Vector2 circlePos = _circleBody.Position*MeterInPixels;
            float circleRot = _circleBody.Rotation;
            _circleSprite.Position = new Vector2f(circlePos.X - (_circleSprite.GetLocalBounds().Width/2f),
                circlePos.Y - (_circleSprite.GetLocalBounds().Height/2f));
            _circleSprite.Rotation = circleRot;

//            Vector2 groundPos = _groundBody.Position*MeterInPixels;
//            _groundSprite.Position = new Vector2f(
//                groundPos.X - (_groundSprite.GetLocalBounds().Width/2f),
//                groundPos.Y );
        }

        private void Render()
        {
            _window.Clear();
            // Draw calls
            _window.SetView(_view);
            _window.Draw(_circleSprite);
            _window.Draw(_groundSprite);
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
