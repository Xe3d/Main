using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Shooter;
using Shooter.PhysicsObjects;
using gravWell.camera;


namespace Farseer331_Setup
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        
        World world;
        List<DrawablePhysicsObject> crateList;
        DrawablePhysicsObject floor;
        DrawablePhysicsObject platform;
        DrawablePhysicsObject wall;
        DrawablePhysicsObject player;
        Texture2D floorTexture;

        KeyboardState prevKeyboardState;
        Random random;
        Player playerObj;
        public Camera2D cam = new Camera2D();

        public const float unitToPixel = 100.0f;
        public const float pixelToUnit = 1 / unitToPixel;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Prefer multisampling to get rid of all the jagged
            // edges of the boxes that we're going to draw
            graphics.PreferMultiSampling = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            playerObj = new Player();
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            random = new Random();



            
            cam.Pos = new Vector2(500.0f, 200.0f);


            // World is the most important farseer object. It's where
            // all the objects should be registered and it handles the 
            // entire simulation via the step function.
            // Here we instantiate it with earth like gravity
            world = new World(new Vector2(0, 9.8f));

            floorTexture = Content.Load<Texture2D>("floor");


          
            floor = new DrawablePhysicsObject(world, floorTexture, new Vector2(GraphicsDevice.Viewport.Width, floorTexture.Height), 1000);
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 50);
            floor.body.BodyType = BodyType.Static;

            wall = new DrawablePhysicsObject(world, floorTexture, new Vector2(10000f, floorTexture.Height), 1000);
            wall.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 400);
          
            wall.body.BodyType = BodyType.Static;
            

            platform = new DrawablePhysicsObject(world, floorTexture, new Vector2(GraphicsDevice.Viewport.Width/2, floorTexture.Height), 1000);
            platform.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height / 2);
            platform.body.BodyType = BodyType.Static;
            

            player = new DrawablePhysicsObject(world,null, new Vector2(36.0f, 48.0f), 1000);
          //  player = BodyFactory.CreateRectangle(world, 36*pixelToUnit, 48*pixelToUnit, 1f);
            player.Position = new Vector2(200.0f, 100.0f);
            player.body.Rotation = 90;
            player.body.BodyType = BodyType.Dynamic;
            
           
            
            crateList = new List<DrawablePhysicsObject>();

            prevKeyboardState = Keyboard.GetState();


            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            Texture2D spriteTexture = Content.Load<Texture2D>("jasperrun");
            playerAnimation.Initialize(spriteTexture, new Vector2(192, 192), 32, 48, 4, 100, Color.White, 1f, true);
       
       //     playerAnimation.Initialize(playerTexture, new Vector2(0, 0), 115, 69, 8, 30, Color.White, 1f, true);
            playerObj.Initialize(playerAnimation, new Vector2(player.Position.X, player.Position.Y));
            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Spawn a crate at a random position at the top of the screen
        /// </summary>
        private void SpawnCrate()
        {
            DrawablePhysicsObject crate;
            crate = new DrawablePhysicsObject(world, Content.Load<Texture2D>("Crate"), new Vector2(50.0f, 50.0f), 0.1f);
            crate.Position = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);

            crateList.Add(crate);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            playerObj.Update(gameTime);
            playerObj.Position.X = player.Position.X;
            playerObj.Position.Y = player.Position.Y;
            
            KeyboardState keyboardState = Keyboard.GetState();
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (keyboardState.IsKeyDown(Keys.Escape)))
                this.Exit();

            if (keyboardState.IsKeyDown(Keys.Right)){
                player.body.LinearVelocity += new Vector2(0.1f, 0.0f);
                
            }
            if (keyboardState.IsKeyDown(Keys.Left)){
                player.body.LinearVelocity += new Vector2(-0.1f, 0.0f);
            }

            
            if (keyboardState.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
            {
                player.body.AngularVelocity += 80.0f;
            }

            if (keyboardState.IsKeyDown(Keys.R))
            {
                
                world.Gravity.X = 9.8f;
                world.Gravity.Y = 0.0f;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {

                cam.Pos += new Vector2(1f, 0f);
                
                
            }

            prevKeyboardState = keyboardState;

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.get_transformation(GraphicsDevice));

            foreach (DrawablePhysicsObject crate in crateList)
            {
                crate.Draw(spriteBatch);
            }

            playerObj.Draw(spriteBatch);
            floor.Draw(spriteBatch);
    //        spriteBatch.Draw(floorTexture,new Rectangle((int)floor.Position.X, (int)floor.Position.Y, (int)floorSize.X, (int)floorSize.Y), Color.White);
      //      spriteBatch.Draw(floorTexture, platform.Position, Color.White);
            platform.Draw(spriteBatch);
            Rectangle wallRect = new Rectangle((int)wall.Position.X, (int)wall.Position.Y, (int)wall.Size.X, (int)wall.Size.Y);
          //  spriteBatch.Draw(wall.texture, wallRect, null, Color.White, wall.body.Rotation, Vector2.Zero, SpriteEffects.None, 0);
            wall.Draw(spriteBatch);
            Console.WriteLine(wall.body.Rotation);
    //        player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}