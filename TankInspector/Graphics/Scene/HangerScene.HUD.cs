using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Graphics.Frameworks;

namespace Smellyriver.TankInspector.Graphics.Scene
{
	internal partial class HangarScene
    {
	    private struct HudLineVertex
        {
            public Vector2              Position;
            public ColorBGRA    Color;
        }

	    private VertexDeclaration _hudVertexDeclaration;
	    private HudLineVertex[] _reticleCrosshairVertices;
	    private HudLineVertex[] _reticleCircleVertices;
	    private Effect _hudEffect;

        private void InitializeHud(Device device)
        {
            _hudVertexDeclaration = new VertexDeclaration(device,new[]
                {
                    new VertexElement(0,0,DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.Position,0),
                    new VertexElement(0,8,DeclarationType.Color,DeclarationMethod.Default,DeclarationUsage.Color,0),
                    VertexElement.VertexDeclarationEnd,
                });

            var color = new ColorBGRA(0, 255, 0, 128);
            _reticleCrosshairVertices = new HudLineVertex[]
            {
                new HudLineVertex { Position = new Vector2(-1,0),Color = color },
                new HudLineVertex { Position = new Vector2(1,0),Color = color},
                new HudLineVertex { Position = new Vector2(0,-1),Color = color},
                new HudLineVertex { Position = new Vector2(0,1),Color = color},
            };

            _hudEffect = Effect.FromFile(device, @"Graphics\Effect\HudEffect.fx", ShaderFlags.None);

            _hudEffect.Technique = _hudEffect.GetTechnique(0);
        }

        private void ResetHud(int width, int height)
        {
            const float circleRadius = 0.7f;

            float radiusV = circleRadius;
            float radiusH = circleRadius;

            if (width > height)
            {
                radiusH *= (float)height / (float)width;
            }
            else
            {
                radiusV *= (float)width / (float)height;
            }

            

            var color = new ColorBGRA(0,255,0,96);
            var vertices = new List<HudLineVertex>();

            double twoPi = (float)Math.PI*2.0f ;
            for (double t = 0; t <= twoPi; t += twoPi / 64)
            {
                vertices.Add(new HudLineVertex
                {
                    Position = new Vector2((float)Math.Sin(t) * radiusH, (float)Math.Cos(t) * radiusV),
                    Color = color,
                });
            }
            _reticleCircleVertices = vertices.ToArray();
        }
        
        private void RenderHud(DrawEventArgs args)
        {
            //var device = Renderer.Device;

            //if (CameraMode == Design.CameraMode.Sniper)
            //{
            //    RenderReticleHUD(device);
            //}
        }

        private void RenderReticleHud(Device device)
        {
            device.VertexDeclaration = _hudVertexDeclaration;
            device.Clear(ClearFlags.ZBuffer | ClearFlags.Stencil, SharpDX.Color.Zero, 1.0f, 0);
            _hudEffect.Begin();
            _hudEffect.BeginPass(0);

            device.DrawUserPrimitives(PrimitiveType.LineList, _reticleCrosshairVertices.Length / 2, _reticleCrosshairVertices);
            device.DrawUserPrimitives(PrimitiveType.LineList, _reticleCircleVertices.Length / 2, _reticleCircleVertices);

            _hudEffect.EndPass();
            _hudEffect.End();
        }
    }
}
