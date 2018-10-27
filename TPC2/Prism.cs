using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPC2
{
    class Prism
    {
        private VertexPositionNormalTexture[] verticesSide;
        private VertexPositionNormalTexture[] verticesTop;
        private VertexBuffer vertexBufferSide;
        private VertexBuffer vertexBufferTop;
        private IndexBuffer indexBufferSide;
        private IndexBuffer indexBufferTop;
        private BasicEffect effectSide;
        private BasicEffect effectTop;
        private Matrix worldMatrix;
        private Vector3 pos;
        private Vector3 movTarget = new Vector3(1f, 0f, 0f);
        private float speed = 0.02f;
        private int NSides;
        private float radius;
        private float height;
        private float yaw = 0.2f;                                                                                                       //velocidade de rotação do prisma
        private GraphicsDevice graphicsDevice;

        public Prism(GraphicsDevice device, int Nsides, float radius, float height, Vector3 startingPos, Texture2D textureSide, Texture2D textureTop, Vector3 lightDir, Vector3 lightDiffuse, Vector3 lightSpec)
        {
            pos = startingPos;                                                                                                          //posição inicial do prisma no espaço
            this.NSides = Nsides;                                                                                                       //numero de lados do prisma
            this.radius = radius;                                                                                                       //raio dos lados do prisma em relação ao centro
            this.height = height;                                                                                                       //altura do prisma

            this.effectSide = new BasicEffect(device);                                                                                  //basic effect do lado
            this.effectTop = new BasicEffect(device);                                                                                   //basic effect do topo
            
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;                                                  //
            effectSide.View = Matrix.CreateLookAt(new Vector3(-2f, 2.0f, -2f), Vector3.Zero, Vector3.Up);                               //
            effectTop.View = Matrix.CreateLookAt(new Vector3(-2f, 2.0f, -2f), Vector3.Zero, Vector3.Up);                                // Calculo do aspectRatio, da view matrix e da projeção do topo e do lado
            effectSide.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), aspectRatio, 0.01f, 1000.0f);      //
            effectTop.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), aspectRatio, 0.01f, 1000.0f);       //
            this.worldMatrix = Matrix.Identity;

            effectSide.VertexColorEnabled = false;                                                                                      //
            effectTop.VertexColorEnabled = false;                                                                                       //
            effectSide.TextureEnabled = true;                                                                                           //Definições dos effects do lado e do topo
            effectTop.TextureEnabled = true;                                                                                            //
            effectSide.Texture = textureSide;                                                                                           //
            effectTop.Texture = textureTop;                                                                                             //

            effectSide.LightingEnabled = true;                                                                                          //
            effectSide.EnableDefaultLighting();                                                                                         //
            effectSide.DirectionalLight0.Enabled = true;                                                                                //
            effectSide.DirectionalLight0.Direction = Vector3.Normalize(lightDir);                                                       //
            effectSide.DirectionalLight0.DiffuseColor = lightDiffuse;                                                                   //
            effectSide.DirectionalLight0.SpecularColor = lightSpec;                                                                     //Ativação de luz e definição da direção, cor difusa e cor specular
            effectTop.LightingEnabled = true;                                                                                           //
            effectTop.EnableDefaultLighting();                                                                                          //
            effectTop.DirectionalLight0.Enabled = true;                                                                                 //
            effectTop.DirectionalLight0.Direction = Vector3.Normalize(lightDir);                                                        //
            effectTop.DirectionalLight0.DiffuseColor = lightDiffuse;                                                                    //
            effectTop.DirectionalLight0.SpecularColor = lightSpec;                                                                      //

            this.graphicsDevice = device;
            CreateGeometry(Nsides, radius, height);
        }

        public void Update(GameTime gameTime, KeyboardState keyboard){
            Matrix rotation = Matrix.CreateRotationY(yaw);                                                                              //Prepara a rotação para a esquerda
            Matrix inverseRot = Matrix.CreateRotationY(-yaw);                                                                           //Prepara a rotação para a direita
            if (keyboard.IsKeyDown(Keys.Left))
                movTarget = Vector3.Transform(movTarget, rotation);                                                                     //Rotação para a esquerda
            if (keyboard.IsKeyDown(Keys.Right))
                movTarget = Vector3.Transform(movTarget, inverseRot);                                                                   //Rotação para a direita

            Vector3 dir = movTarget;                                                                                                    
            dir.Normalize();                                                                                                            //Normaliza o vetor de rotação

            rotation.Forward = dir;                                                                                                     //Dá direção da frente no vetor de rotação (pode usar o da rotação para esquerda mesmo que a rotação seja para a direita, pois não faz mais rotação a volta da coordenada y)
            rotation.Up = Vector3.UnitY;                                                                                                //Direção vertical para cima
            rotation.Right = Vector3.Cross(dir, Vector3.UnitY);                                                                         //Direção do lado direito, feito a partir de um Cross entre a direção e a direção vertical

            worldMatrix = rotation * Matrix.CreateTranslation(pos);                                                                     //Atualiza a worldMatrix a partir da rotação e de uma translação criada a partir da posição

            if (keyboard.IsKeyDown(Keys.Up))
                pos = pos + movTarget * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;                                      //Mexe para a frente
            if (keyboard.IsKeyDown(Keys.Down))
                pos = pos - movTarget * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;                                      //Mexe para tras
        }

        public void CreateGeometry(int Nsides, float radius, float height)
        {
            if (Nsides < 3)                                                                                                             //Não permite que sejam criados prismas com menos de 3 lados 
                return;
            int vertexCountSide = (Nsides + 1) * 2;                                                                                     //Calculo da quantidade de vetores dos lados
            int vertexCountTop = Nsides + 1;                                                                                            //Calculo da quantidade de vetores do topo

            verticesSide = new VertexPositionNormalTexture[vertexCountSide];
            verticesTop = new VertexPositionNormalTexture[vertexCountTop];

            for(int i = 0; i < Nsides + 1; i++){                                                                                        //Calcula a posição, normais e posição de textura dos vertices do topo e dos lados
                float angle = (float)(Math.PI * 2 / Nsides) * i;
                float x = (float)Math.Cos(angle) * radius + pos.X;
                float z = (float)-Math.Sin(angle) * radius + pos.Z;

                float textureX = 0.5f + 0.5f * (float)Math.Cos(angle);
                float textureY = 0.5f - 0.5f * (float)Math.Sin(angle);

                verticesSide[i * 2] = new VertexPositionNormalTexture(new Vector3(x, pos.Y, z), Vector3.Normalize(new Vector3(x, 0, z)), new Vector2((float)i / (float)NSides, 1.0f));
                verticesSide[i * 2 + 1] = new VertexPositionNormalTexture(new Vector3(x, pos.Y + height, z), Vector3.Normalize(new Vector3(x, 0, z)), new Vector2((float)i / (float)NSides, 0.0f));
                if(i == NSides){                                                                                                        //Última posição do topo é o centro
                    verticesTop[verticesTop.Length - 1] = new VertexPositionNormalTexture(new Vector3(pos.X, height + pos.Y, pos.Z), Vector3.Normalize(new Vector3(pos.X, height + pos.Y, pos.Z)), new Vector2(0.5f, 0.5f));
                    continue;
                }
                verticesTop[i] = new VertexPositionNormalTexture(new Vector3(x, pos.Y + height, z), Vector3.Up, new Vector2(textureX, textureY));
            }
            vertexBufferSide = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexCountSide, BufferUsage.None);
            vertexBufferSide.SetData<VertexPositionNormalTexture>(verticesSide);

            vertexBufferTop = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexCountTop, BufferUsage.None);
            vertexBufferTop.SetData<VertexPositionNormalTexture>(verticesTop);

            int indexCountSide = (Nsides + 1) * 2;
            short[] indices = new short[indexCountSide];
            for(int i = 0; i < Nsides + 1; i++){                                                                                        //Calculo dos indices do lado

                indices[2 * i] = (short)(2 * i);
                indices[2 * i + 1] = (short)(2 * i + 1);
            }
            indexBufferSide = new IndexBuffer(graphicsDevice, typeof(short), indexCountSide, BufferUsage.None);
            indexBufferSide.SetData<short>(indices);

            int indexCountTop = 2 * (Nsides + 1);
            short[] indicesTop = new short[indexCountTop];
            for(int i = 0; i < Nsides + 1; i++){                                                                                        //Calculo dos indices do topo
                indicesTop[2 * i] = (short)(i % Nsides);
                indicesTop[2 * i + 1] = (short)(verticesTop.Length - 1);
            }
            indexBufferTop = new IndexBuffer(graphicsDevice, typeof(short), indexCountTop, BufferUsage.None);
            indexBufferTop.SetData<short>(indicesTop);
        }

        public void Draw(GraphicsDevice device, Matrix viewMatrix)
        {
            if (NSides < 3)
                return;
            effectSide.World = worldMatrix;                                                                                             //Atualização de worldMatrix, viewMatrix, vertexBuffer e indexBuffer dos lados e draw dos mesmos
            effectSide.View = viewMatrix;                                                                                               //
            effectSide.CurrentTechnique.Passes[0].Apply();                                                                              //
            device.SetVertexBuffer(vertexBufferSide);                                                                                   //
            device.Indices = indexBufferSide;                                                                                           //
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, 2 * NSides);                                                //

            effectTop.World = worldMatrix;                                                                                              //Atualização de worldMatrix, viewMatrix, vertexBuffer e indexBuffer do topo e draw do próprio
            effectTop.View = viewMatrix;                                                                                                //
            effectTop.CurrentTechnique.Passes[0].Apply();                                                                               //
            device.SetVertexBuffer(vertexBufferTop);                                                                                    //
            device.Indices = indexBufferTop;                                                                                            //
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, NSides * 2);                                                //
        }
    }
}

