using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPC2
{
    class StaticCamera
    {
        public Matrix viewMatrix;

        public StaticCamera(Vector3 pos, Vector3 target){
            viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);
        }
    }
}
