using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPC2
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Prism prism;
        Terrain terrain;
        StaticCamera camera;
        Vector3 lightDir = new Vector3(-1, -1, 0);
        Vector3 lightDiffuse = new Vector3(232f / 255f, 53f / 255f, 223f / 255f);
        Vector3 lightSpec = new Vector3(1f / 255f, 1f / 255f, 1f / 255f);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            terrain = new Terrain(Content.Load<Texture2D>("plane"), GraphicsDevice, 3f, Vector3.Zero);
            prism = new Prism(GraphicsDevice, 32, 1f, 3f, Vector3.Zero, Content.Load<Texture2D>("beans2"), Content.Load<Texture2D>("gravel"), lightDir, lightDiffuse, lightSpec);
            camera = new StaticCamera(new Vector3(2, 4, 2), Vector3.Zero);
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            prism.Update(gameTime, Keyboard.GetState());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            terrain.Draw(GraphicsDevice, camera.viewMatrix);
            prism.Draw(GraphicsDevice, camera.viewMatrix);
            base.Draw(gameTime);
        }
    }
}
