using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPC2
{
    class Terrain
    {
        Texture2D texture;
        VertexBuffer vertexBuffer;
        BasicEffect effect;
        Matrix worldMatrix;
        int vertexCount;
        VertexPositionTexture[] vertices;

        public Terrain(Texture2D texture, GraphicsDevice graphicsDevice, float radius, Vector3 startingPos){
            effect = new BasicEffect(graphicsDevice);
            float aspectRatio = (float)graphicsDevice.Viewport.Width / graphicsDevice.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(-2f, 2.0f, -2f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), aspectRatio, 0.01f, 1000.0f);
            this.worldMatrix = Matrix.Identity;
            effect.LightingEnabled = false;     
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            this.texture = texture;
            CreateGeometry(graphicsDevice, radius, startingPos);
        }

        private void CreateGeometry(GraphicsDevice graphicsDevice, float radius, Vector3 pos){
            vertexCount = 4;
            vertices = new VertexPositionTexture[vertexCount];
            vertices[0] = new VertexPositionTexture(new Vector3(-radius + pos.X, 0 + pos.Y, -radius + pos.Z), new Vector2(0, 0));
            vertices[1] = new VertexPositionTexture(new Vector3(+radius + pos.X, 0 + pos.Y, -radius + pos.Z), new Vector2(1, 0));
            vertices[2] = new VertexPositionTexture(new Vector3(-radius + pos.X, 0 + pos.Y, +radius + pos.Z), new Vector2(0, 1));
            vertices[3] = new VertexPositionTexture(new Vector3(+radius + pos.X, 0 + pos.Y, +radius + pos.Z), new Vector2(1, 1));
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionTexture>(vertices);
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix viewMatrix){
            // World Matrix
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Texture = this.texture;
            // Indica o efeito para desenhar os eixos
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }
    }
}
