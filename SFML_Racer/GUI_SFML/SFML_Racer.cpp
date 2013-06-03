// Basic layout

#include <SFML\Graphics.hpp>
#include <algorithm>
#include <sstream>

using namespace sf;

class Game
{

private:
	static const int FRAMES_PER_SECOND = 60;
	static const int MAX_FRAMESKIP = 10;
	static const int width = 640;
	static const int height = 480;	

	RenderWindow window;
	Font font;
	Text fps;
	Texture buttons [2];
	Sprite sprites [2];
	int spriteToDisplay;

	Vector2i mousePos;
	Vector2f spritePos;
	FloatRect textureInfo;

	Time time;
	Int32 updateTime;	

	enum states {INTRO, PLAYING};

	int gameState;

public:
	Game()
	{
		
	}

	bool initialize()
	{
		VideoMode videoMode(width, height);
		window.create(videoMode, "GUI");
		window.setVerticalSyncEnabled(true);
		window.setFramerateLimit(FRAMES_PER_SECOND);

		if (!font.loadFromFile("GameResources/arial.ttf"))
			return false;

		fps.setString("0");
		fps.setFont(font);
		fps.setCharacterSize(30);
		fps.setPosition(fps.getGlobalBounds().width/2,40);
		fps.setColor(Color(52,0,100,50));

		if (!buttons[0].loadFromFile("GameResources/play.png"))
			return false;
		if (!buttons[1].loadFromFile("GameResources/playOver.png"))
			return false;

		spritePos = Vector2f(window.getSize().x/2 - buttons[0].getSize().x/2, window.getSize().y/2 - buttons[0].getSize().y/2);
		
		sprites[0].setTexture(buttons[0]);
		sprites[0].setPosition(spritePos);
		sprites[1].setTexture(buttons[1]);
		sprites[1].setPosition(spritePos);

		textureInfo = sprites[0].getGlobalBounds();

		spriteToDisplay = 0;

		gameState=INTRO;
			
		return true;
	}

	int update()
	{
		Clock renderClock, updateClock;
		while (window.isOpen())
		{
			time = renderClock.getElapsedTime();
			float fFps = 1000000/time.asMicroseconds();
			std::stringstream s;
			s << fFps << " fps";
			fps.setString(s.str());
			renderClock.restart();

			const Int64 frameTime = 1000000/FRAMES_PER_SECOND;
			Clock c;
			Time t = c.getElapsedTime();
			Int64 nextFrameTime = t.asMicroseconds() + frameTime;

			int loops = 0;
			while ( t.asMicroseconds() < nextFrameTime && loops < MAX_FRAMESKIP)
			{
				processEvents(); // user input
				processMousePosition();
				updateTime = updateClock.restart().asMilliseconds();
				
				//Update here - nb - check game state i.e INTRO, PLAYING ...


				t = c.getElapsedTime();
				loops++;
			}

			draw();

		}

		return EXIT_SUCCESS;
	}

	void processMousePosition()
	{

		mousePos = Mouse::getPosition( window );
		
		// Check if hovering over button
		if ((mousePos.x >= spritePos.x && mousePos.x <= (spritePos.x + textureInfo.width)) && (mousePos.y >= spritePos.y && mousePos.y <= (spritePos.y + textureInfo.height) ))
		{			
			spriteToDisplay = 1;
		}
		else
		{
			spriteToDisplay = 0;
		}

	}

	void processEvents()
	{
		Event event;
		while (window.pollEvent(event))
		{
			if ( (event.type == Event::Closed) ||
				((event.type == Event::KeyPressed) && (event.key.code==Keyboard::Escape)) )
				window.close();    
			else
				if ((event.type == Event::KeyPressed) && (gameState == INTRO))
					gameState=PLAYING;
		}

		//move player 1 pad
		if (Keyboard::isKeyPressed(Keyboard::Up))
		{
			//player1.move(0, -moveDistance*updateTime/50);
		}
		else if (Keyboard::isKeyPressed(Keyboard::Down))
		{
			//player1.move(0, moveDistance*updateTime/50.0);
		}
	}

	void draw()
	{
		window.clear(Color::White);
		switch(gameState)
		{
		case INTRO:
			window.draw(fps);
			window.draw(sprites[spriteToDisplay]);
			break;
		case PLAYING:			
			window.draw(fps);
			break;		
		}
		window.display();
	}	

};


int main()
{	

	Game game;
    if (!game.initialize())
        return EXIT_FAILURE;
    return game.update();

}
