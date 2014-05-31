using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace SFMLStarter2
{
    /// <summary>
    /// Port of the C++ Animation class from chapter 8 of "SFML Game Development" to C#
    /// http://www.packtpub.com/sfml-game-development/book
    /// </summary>
    public class Animation : Transformable, Drawable
    {
        private readonly Sprite _sprite;
        private Vector2i _frameSize;
        private int _numFrames;
        private int _currentFrame;
        private double _duration;
        private double _elapsedTime;
        private bool _repeat;

        public Animation()
        {
            _sprite = new Sprite();
        }
        public Animation(Texture texture)
        {
            _sprite = new Sprite(texture);
            
            _numFrames = 0;
            _currentFrame = 0;
            _duration = 0;
            _elapsedTime = 0;
            _repeat = false;
        }

        public void SetTexture(Texture texture)
        {
            _sprite.Texture = texture;
        }

        public Texture GetTexture()
        {
            return _sprite.Texture;
        }

        public void SetFrameSize(Vector2i frameSize)
        {
            _frameSize = frameSize;
            _sprite.TextureRect = new IntRect(0, 0, _frameSize.X, _frameSize.Y);
        }

        public Vector2i GetFrameSize()
        {
            return _frameSize;
        }

        public void SetNumFrames(int numFrames)
        {
            _numFrames = numFrames;
        }

        public int GetNumFrames()
        {
            return _numFrames;
        }

        public void SetDuration(double durationinSeconds)
        {
            _duration = durationinSeconds;
        }

        public double GetDuration()
        {
            return _duration;
        }

        public void SetRepeating(bool flag)
        {
            _repeat = flag;
        }

        public bool IsRepeating()
        {
            return _repeat;
        }

        public void Restart()
        {
            _currentFrame = 0;
        }

        public bool IsFinished()
        {
            return _currentFrame >= _numFrames;
        }

        public FloatRect GetLocalBounds()
        {
            return new FloatRect(Origin.X, Origin.Y, GetFrameSize().X, GetFrameSize().Y);
        }

        public FloatRect GetGlobalBounds()
        {
            return Transform.TransformRect(GetLocalBounds());
        }

        public void Update(Double delta)
        {
            Double timePerFrame = _duration/(float)_numFrames;
            _elapsedTime += delta;

            var textureBounds = _sprite.Texture.Size;

            IntRect textureRect = _sprite.TextureRect;

            if (_currentFrame == 0)
                textureRect = new IntRect(0, 0, _frameSize.X, _frameSize.Y);

            while (_elapsedTime >= timePerFrame && (_currentFrame <= _numFrames || _repeat))
            {
                textureRect.Left += textureRect.Width;

                if (textureRect.Left + textureRect.Width > textureBounds.X)
                {
                    // move it down one line
                    textureRect.Left = 0;
                    textureRect.Top += textureRect.Height;
                }

                _elapsedTime -= timePerFrame;
                if (_repeat)
                {
                    _currentFrame = (_currentFrame + 1) % _numFrames;

                    if (_currentFrame == 0)
                        textureRect = new IntRect(0, 0, _frameSize.X, _frameSize.Y);
                }
                else
                {
                    _currentFrame++;
                }

                _sprite.TextureRect = textureRect;
            }

        }
        
        public void Draw(RenderTarget target, RenderStates states)
        {
            
            target.Draw(_sprite, states);
        }

        public void SetPosition(Vector2f vector2F)
        {
            _sprite.Position = new Vector2f(vector2F.X - _frameSize.X/2f, vector2F.Y - _frameSize.Y/2f);
        }
    }
}
